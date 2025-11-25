using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ContractorSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ContractorSetup
{
    public class UpsertContractor
    {

        public class UpsertContractorCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Contractor { get; set; }
            public DateTime? DateAdded { get; set; }
            public DateTime? DateUpdated { get; set; }
            public int? channelId { get; set; }
            public int? PivotId { get; set; }

        }

        public class Handler : IRequestHandler<UpsertContractorCommand, Result>
        {
            private readonly MisDbContext _context;


            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertContractorCommand command, CancellationToken cancellationToken)
            {

                var listContractor = new List<ContractorChannel>();

                var contractorExist = await _context.ContractorChannels.FirstOrDefaultAsync(x => x.Contractor.ContractorName == command.Contractor && x.ChannelId == command.channelId, cancellationToken);

                if (contractorExist != null)
                {
                    return Result.Failure(ContractorError.ContractorAlreadyExist(command.Contractor));
                }

                var contractor = await _context.ContractorChannels.Include(x => x.Contractor).FirstOrDefaultAsync(x => x.Id == command.PivotId, cancellationToken);
                if (contractor != null)
                {
                    if (contractor.Contractor.ContractorName == command.Contractor && contractor.ChannelId == command.channelId)
                    {
                        return Result.Failure(ContractorError.ContractorNochanges());
                    }

                    contractor.Contractor.ContractorName = command.Contractor;
                    contractor.Contractor.DateUpdated = DateTime.Now;
                    contractor.ChannelId = command.channelId;

                    await _context.SaveChangesAsync(cancellationToken);

                }
                else
                {
                    var addContractor = new Contractor
                    {
                        ContractorName = command.Contractor,
                        DateAdded = DateTime.Now,
                        IsActive = true
                    };

                    await _context.Contractors.AddAsync(addContractor, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    var channelNoExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.channelId, cancellationToken);

                    if (channelNoExist == null)
                    {
                        return Result.Failure(ServiceError.ChannelNotExist());
                    }

                    var serviceName = await _context.Contractors.FirstOrDefaultAsync(x => x.ContractorName == command.Contractor && x.Id == addContractor.Id, cancellationToken);
                    var serviceChannel = await _context.ContractorChannels.Include(x => x.Channel).FirstOrDefaultAsync(x => x.ChannelId == command.channelId && x.ContractorId == serviceName.Id);

                    if (serviceChannel != null)
                    {

                        return Result.Failure(ServiceError.ContractorAlreadyExists());
                    }

                    else
                    {
                        var addContractorChannel = new ContractorChannel
                        {
                            ContractorId = serviceName.Id,
                            ChannelId = command.channelId,
                        };

                        await _context.ContractorChannels.AddAsync(addContractorChannel, cancellationToken);
                    }

                    

                }




                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
