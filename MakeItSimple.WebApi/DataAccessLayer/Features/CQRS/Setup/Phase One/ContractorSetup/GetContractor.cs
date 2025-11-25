using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ContractorSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup
{
    public class GetContractor
    {
        public class GetContractorResult
        {
            public int Id { get; set; }
            public string contractor { get; set; }
            public string ChannelName { get; set; }
            public int? ChannelId { get; set; }
            public bool? Is_Active { get; set; }
            public int? PivotId { get; set; }
        }

        public class GetContractorQuery : UserParams, IRequest<PagedList<GetContractorResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetContractorQuery, PagedList<GetContractorResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<PagedList<GetContractorResult>> Handle(GetContractorQuery request, CancellationToken cancellationToken)
            {
                IQueryable<ContractorChannel> contractorQuery = _context.ContractorChannels
                .AsNoTracking().AsQueryable();


                if (!string.IsNullOrEmpty(request.Search))
                {
                    contractorQuery = contractorQuery.Where(x => x.Contractor.ContractorName.ToLower().Contains(request.Search)
                    || x.Channel.ChannelName.ToLower().Contains(request.Search));
                }

                if (request.Status != null)
                {
                    contractorQuery = contractorQuery.Where(x => x.Contractor.IsActive == request.Status);
                }

                var result = contractorQuery.Select(x => new GetContractorResult
                {
                    Id = x.Contractor.Id,
                    contractor = x.Contractor.ContractorName,
                    Is_Active = x.Contractor.IsActive,
                    ChannelName = x.Channel.ChannelName,
                    ChannelId = x.ChannelId,
                    PivotId = x.Id,
                });

                return await PagedList<GetContractorResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }



        }

    }
}
