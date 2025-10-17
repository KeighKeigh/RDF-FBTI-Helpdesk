using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport
{
    public partial class TransferTicketReports
    {

         public class Handler : IRequestHandler<TransferTicketReportsQuery, PagedList<TransferTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<TransferTicketReportsResult>> Handle(TransferTicketReportsQuery request, CancellationToken cancellationToken)
            {

                IQueryable<TransferTicketConcern> _transferQuery = _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.TransferByUser)
                    .Include(x => x.TransferToUser)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.User)
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestConcern)
                    .AsSplitQuery().OrderBy(x => x.Current_Target_Date);

                if (request.ServiceProvider is not null)
                {
                    _transferQuery = _transferQuery.Where(x => x.TicketConcern.RequestConcern.ServiceProviderId == request.ServiceProvider);

                    if (request.Channel is not null)
                    {
                        _transferQuery = _transferQuery.Where(x => x.TicketConcern.RequestConcern.ChannelId == request.Channel);

                        if (request.UserId is not null)
                        {
                            _transferQuery = _transferQuery.Where(x => x.TransferBy == request.UserId);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    _transferQuery = _transferQuery
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.TransferByUser.Fullname.Contains(request.Search));
                }

                var results = _transferQuery
                    .Where(x => x.IsTransfer == true && x.TicketConcern.UserId != null)
                     .Where(x => x.TransferAt.Value.Date >= request.Date_From.Value.Date && x.TransferAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new TransferTicketReportsResult
                    {
                        TicketConcernId = x.TicketConcernId,
                        TransferTicketId = x.Id,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Transfered_By = x.TransferByUser.Fullname,
                        Transfered_To = x.TransferToUser.Fullname,
                        Requested_Date = x.TicketConcern.CreatedAt,
                        New_Target_Date = x.Current_Target_Date.Value.Date,
                        Previous_Target_Date = x.TicketConcern.TargetDate,
                        Approved_At = x.TransferAt,
                        Transfer_Remarks = x.TransferRemarks,
                        Remarks = x.TransferRemarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        ApprovedBy = x.ApprovedBy,
                        ServiceProviderId = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ServiceProviderName = x.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        ChannelName = x.TicketConcern.RequestConcern.Channel.ChannelName




                    });


                return await PagedList<TransferTicketReportsResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
