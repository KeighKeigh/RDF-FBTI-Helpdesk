 using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup
{
    public class UpsertCategoryConcern
    {

        public class UpsertCategoryConcernCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string concernCategory { get; set; }
            public DateTime? DateAdded { get; set; }
            public DateTime? DateUpdated { get; set; }
            public int? channelId { get; set; }
            public int? PivotId { get; set; }


        }

        public class Handler : IRequestHandler<UpsertCategoryConcernCommand, Result>
        {
            private readonly MisDbContext _context;
            

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(UpsertCategoryConcernCommand command, CancellationToken cancellationToken)
            {

                var listDelete = new List<CategoryConcernChannel>();
                var listCategoryConcern = new List<CategoryConcernChannel>();


                var categoryConcernExist = await _context.CategoryConcernChannels.FirstOrDefaultAsync(x => x.CategoryConcern.CategoryConcernName == command.concernCategory && x.ChannelId == command.channelId, cancellationToken);

                if (categoryConcernExist != null)
                {
                    return Result.Failure(CategoryConcernError.CategoryConcernAlreadyExist(command.concernCategory));
                }
                //var categoryConcernChannel = await _context.CategoryConcernChannels.FirstOrDefaultAsync(x => x.Id == command.PivotId, cancellationToken);
                var categoryConcern = await _context.CategoryConcernChannels.Include(x => x.CategoryConcern).FirstOrDefaultAsync(x => x.Id == command.PivotId, cancellationToken);
                if (categoryConcern != null)
                {
                    if (categoryConcern.CategoryConcern.CategoryConcernName == command.concernCategory && categoryConcern.ChannelId == command.channelId)
                    {
                        return Result.Failure(CategoryConcernError.CategoryConcernNochanges());
                    }

                    categoryConcern.CategoryConcern.CategoryConcernName = command.concernCategory;
                    categoryConcern.CategoryConcern.DateUpdated = DateTime.Now;
                    categoryConcern.ChannelId = command.channelId;

                    await _context.SaveChangesAsync(cancellationToken);
                    
                }
                else
                {
                    var addCategoryConcern = new CategoryConcern
                    {
                        CategoryConcernName = command.concernCategory,
                        DateAdded = DateTime.Now,
                        IsActive = true
                    };

                    await _context.CategoryConcerns.AddAsync(addCategoryConcern, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);


                    var channelNoExist = await _context.Channels.FirstOrDefaultAsync(x => x.Id == command.channelId, cancellationToken);

                    if (channelNoExist == null)
                    {
                        return Result.Failure(ServiceError.ChannelNotExist());
                    }


                    var serviceName = await _context.CategoryConcerns.FirstOrDefaultAsync(x => x.CategoryConcernName == command.concernCategory && x.Id == addCategoryConcern.Id, cancellationToken);
                    var serviceChannel = await _context.CategoryConcernChannels.Include(x => x.Channel).FirstOrDefaultAsync(x => x.ChannelId == command.channelId && x.CategoryConcernId == serviceName.Id);

                    if (serviceChannel != null)
                    {
                        return Result.Failure(ServiceError.CategoryConcernAlreadyExists());

                    }
                    else
                    {   
                        var addServiceProviderChannel = new CategoryConcernChannel
                        {
                            CategoryConcernId = serviceName.Id,
                            ChannelId = command.channelId,
                        };

                        await _context.CategoryConcernChannels.AddAsync(addServiceProviderChannel, cancellationToken);
                    }

                    
                    
                }


                
                    

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }

        
    }
}
