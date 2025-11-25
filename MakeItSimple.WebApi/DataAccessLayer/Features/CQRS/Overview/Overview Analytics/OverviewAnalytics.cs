using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Overview.Overview_Analytics
{
    public partial class OverviewAnalytics
    {

        public class Handler : IRequestHandler<OverviewAnalyticsQuery, Result>
        {
            private readonly MisDbContext context;

            public Handler(MisDbContext context)
            {
                this.context = context;
            }

            public async Task<Result> Handle(OverviewAnalyticsQuery request, CancellationToken cancellationToken)
            {
                var query = context.TicketConcerns.AsNoTrackingWithIdentityResolution()
                    .Include(x => x.User)
                    .ThenInclude(x => x.OneChargingMIS)
                    .Where(x => x.UserId != null && x.IsDateApproved == true);

                var totalTickets = await query.CountAsync();
                var totalDelay = await query.CountAsync(x => x.IsClosedApprove == true && x.Closed_At > x.TargetDate.Value.Date );
                var totalClosed = await query.CountAsync(x => x.Closed_At != null);
                //EF.Functions.DateDiffDay(x.TicketConcern.DateApprovedAt.Value.Date, x.ClosingAt.Value.Date)
                var completedTicket = await query.Where(x => x.Closed_At != null).CountAsync();
                var avgResolution = await query
       .Where(x => x.Closed_At != null)
       .AverageAsync(x =>
           EF.Functions.DateDiffDay(
               x.DateApprovedAt.Value.Date,
               x.Closed_At.Value.Date
           )
       );

                var completionRate = Math.Round(((decimal)completedTicket / totalTickets) * 100,2);
                //var totalAvgResolution = query.Where(x => x.Closed_At != null);
                //var avgResolution = totalAvgResolution.Sum(x => EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date));
                if (request.DateFrom is not null && request.DateTo is not null) 
                    query =  query.Where(x => x.CreatedAt.Date <= request.DateFrom.Value.Date && x.CreatedAt >= request.DateTo.Value.Date);

                
                var results = query 
                    .GroupBy(x => x.User.DepartmentId)
                    .Select(x => new OverviewAnalyticsResult
                    {
                        Department = x.First().User.OneChargingMIS.department_name,
                        TotalTicket = totalTickets,
                        TotalClosed = totalClosed,
                        TotalDelay = totalDelay,
                        CompletedTickets = completedTicket,
                        AvgResolutionTime = avgResolution,
                        CompletionRate = completionRate,
                        OverviewAnalyticsDetails = x.GroupBy(x => x.UserId)
                        .Select(x => new OverviewAnalyticsResult.OverviewAnalyticsDetail
                        {
                            Id = x.Key.Value,
                            FullName = x.First().User.Fullname,
                            NumberOfTicket = x.Count(),
                            OpenTicketCount = x.Where(x => x.IsApprove == true && x.ConcernStatus == TicketingConString.OnGoing && x.IsClosedApprove == null && x.IsTransfer == null && x.OnHold == null || x.IsTransfer != false
                                && x.IsClosedApprove == null && x.OnHold == null && x.IsApprove == true).Count(),
                            OnTimeTicketCount = x.Count(x => x.IsClosedApprove == true && x.Closed_At.Value.Date <= x.TargetDate.Value.Date),
                            PercentageTicket =Math.Round(((decimal)x.Count() / totalTickets) * 100,2),
                            NumberOfDelay = x.Count(x => x.IsClosedApprove == true && x.Closed_At.Value.Date > x.TargetDate.Value.Date),
                            DelayTicketPercentage = Math.Round(((decimal)x.Count(x => x.IsClosedApprove == true && x.Closed_At.Value.Date > x.TargetDate.Value.Date) / x.Count()) * 100,2),

                        }).OrderByDescending(x => x.PercentageTicket).ToList(),
                    });

                return Result.Success(results);
            }
        }


    }
}
