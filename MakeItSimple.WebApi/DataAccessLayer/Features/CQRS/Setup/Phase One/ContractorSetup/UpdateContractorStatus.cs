using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup
{
    public class UpdateContractorStatus
    {
        public class UpdateContractorStatusResult
        {
            public int Id { get; set; }
            public bool? Is_Active { get; set; }

        }

        public class UpdateContractorStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<UpdateContractorStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpdateContractorStatusCommand command, CancellationToken cancellationToken)
            {
                var contractor = await _context.Contractors.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
                if (contractor == null)
                {
                    return Result.Failure(ContractorError.ContractorNotExist());
                }

                contractor.IsActive = !contractor.IsActive;
                contractor.DateUpdated = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);

                var result = new UpdateContractorStatusResult
                {
                    Id = contractor.Id,
                    Is_Active = contractor.IsActive,
                };

                return Result.Success(result);
            }
        }
    }
}
