using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup
{
    public class GetCategoryConcern
    {
        public class GetCategoryConcernResult
        {
            public int Id { get; set; }
            public string categoryConcern { get; set; }
            public string ChannelName { get; set; }
            public int? ChannelId { get; set; }
            public bool? Is_Active { get; set; }
            public int? PivotId { get; set; }
        }

        public class GetCategoryConcernQuery : UserParams, IRequest<PagedList<GetCategoryConcernResult>>
        {
            public string Search {  get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetCategoryConcernQuery, PagedList<GetCategoryConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<PagedList<GetCategoryConcernResult>> Handle(GetCategoryConcernQuery request, CancellationToken cancellationToken)
            {
                IQueryable<CategoryConcernChannel> categoryConcernsQuery = _context.CategoryConcernChannels
                .AsNoTracking().AsQueryable();


                if (!string.IsNullOrEmpty(request.Search))
                {
                    categoryConcernsQuery = categoryConcernsQuery.Where(x => x.CategoryConcern.CategoryConcernName.ToLower().Contains(request.Search)
                    || x.Channel.ChannelName.ToLower().Contains(request.Search));
                }

                if (request.Status != null)
                {
                    categoryConcernsQuery = categoryConcernsQuery.Where(x => x.CategoryConcern.IsActive == request.Status);
                }

                var result = categoryConcernsQuery.Select(x => new GetCategoryConcernResult
                {
                    Id = x.CategoryConcern.Id,
                    categoryConcern = x.CategoryConcern.CategoryConcernName,
                    Is_Active = x.CategoryConcern.IsActive,
                    ChannelId = x.ChannelId,
                    ChannelName = x.Channel.ChannelName,
                    PivotId = x.Id
                });

                return await PagedList<GetCategoryConcernResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }

            
            
        }
    }
}
