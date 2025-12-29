using Azure.Core;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Linq;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.BackjobReport
{
    public class BackJobReportHandler
    {

        public class BackJobReportQuery : UserParams, IRequest<PagedList<BackJobReportResult>>
        {
            public string Search { get; set; }
            public int? ServiceProvider { get; set; }
            public int? Channel { get; set; }
            public Guid? UserId { get; set; }
        }


        public class BackJobReportResult
        {
            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? OriginalTicketNo { get; set; }
            public string BackJobTicketNumber { get; set; }
            public string ConcernDescription { get; set; }
            public string Reason { get; set; }
            public string IssueHandler { get; set; }
            public string Channel { get; set; }
            public string RequestorName { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string Business_Unit_Code { get; set; }
            public string Business_Unit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public string BackJobDate { get; set; }
            public string TargetDate { get; set; }
            public int? Backjobs { get; set; }
        }

        public class DescendantInfo
        {
            public int TicketNo { get; set; }
            public int BackJobId { get; set; }
            public DateTime? CreatedAt { get; set; }
            public DateTime? TargetDate { get; set; }
        }

        public class Handler : IRequestHandler<BackJobReportQuery, PagedList<BackJobReportResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<BackJobReportResult>> Handle(BackJobReportQuery query, CancellationToken cancellationToken)
            {


                var allRequestConcerns = await _context.TicketConcerns
                    .AsNoTracking()
                    .Where(x => x.IsActive)
                    .Select(x => new { x.Id, x.RequestConcern.BackJobId, x.TargetDate, x.CreatedAt })
                    .ToListAsync();


                var reworkIds = allRequestConcerns
                    .Where(x => x.BackJobId.HasValue)
                    .Select(x => x.BackJobId)
                    .ToHashSet();

                var rootTicketIds = allRequestConcerns
                    .Where(x => reworkIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToList();


                var childrenLookup = allRequestConcerns
        .Where(x => x.BackJobId.HasValue)
        .ToLookup(x => x.BackJobId.Value, x => x.Id);

                var rootTicketTargetDates = allRequestConcerns
                    .Where(x => reworkIds.Contains(x.BackJobId))
                    .ToList();



                var rootTicketDescendants = new Dictionary<int, List<int>>();
                        
                foreach (var rootId in rootTicketIds)
                {
                    var allDescendants = new List<int>();
                    if (!allDescendants.Contains(rootId))
                    {
                        var queue = new Queue<int>();
                        var visited = new HashSet<int>();


                        foreach (var childId in childrenLookup[rootId])
                        {
                            queue.Enqueue(childId);
                            visited.Add(childId);
                            allDescendants.Add(childId);
                        }


                        while (queue.Count > 0)
                        {
                            var currentId = queue.Dequeue();
                            foreach (var childId in childrenLookup[currentId])
                            {
                                if (visited.Add(childId))
                                {
                                    queue.Enqueue(childId);
                                    allDescendants.Add(childId);
                                }
                            }
                        }
                    }

                    rootTicketDescendants[rootId] = allDescendants;
                }

                var allDescendantIds = rootTicketDescendants.Keys.ToList();
                var removeticketDuplicate = rootTicketIds
                    .Where(id => !allDescendantIds.Contains(id))
                    .ToList();


                var ticketReport =  _context.TicketConcerns
                     .Include(t => t.RequestConcern)
                       .ThenInclude(rc => rc.OneChargingMIS)
                     .Include(t => t.RequestConcern)
                       .ThenInclude(rc => rc.Channel)
                     .Include(t => t.User)
                     .Include(t => t.RequestorByUser)
                     .AsSplitQuery()
                     .Where(t => rootTicketIds.Contains(t.Id) && t.RequestConcern.BackJobId == null)
                     .Select(t => new BackJobReportResult
                     {
                         Year = t.TargetDate.Value.Year,
                         Month = t.TargetDate.Value.Month,
                         OriginalTicketNo = t.Id,
                         ConcernDescription = t.RequestConcern.Concern,
                         Reason = t.Reason,
                         IssueHandler = t.User.Fullname,
                         Channel = t.RequestConcern.Channel.ChannelName,
                         RequestorName = t.AddedByUser.Fullname,
                         CompanyCode = t.RequestConcern.OneChargingMIS.company_code,
                         CompanyName = t.RequestConcern.OneChargingMIS.company_name,
                         Business_Unit_Code = t.RequestConcern.OneChargingMIS.business_unit_code,
                         Business_Unit_Name = t.RequestConcern.OneChargingMIS.business_unit_name,
                         Department_Code = t.RequestConcern.OneChargingMIS.department_code,
                         Department_Name = t.RequestConcern.OneChargingMIS.department_name,
                         Unit_Code = t.RequestConcern.OneChargingMIS.department_unit_code,
                         Unit_Name = t.RequestConcern.OneChargingMIS.department_unit_name,
                         SubUnit_Code = t.RequestConcern.OneChargingMIS.sub_unit_code,
                         SubUnit_Name = t.RequestConcern.OneChargingMIS.sub_unit_name,
                         Location_Code = t.RequestConcern.OneChargingMIS.location_code,
                         Location_Name = t.RequestConcern.OneChargingMIS.location_name,                

                     }).AsQueryable();

                var pagedResult = await PagedList<BackJobReportResult>.CreateAsync(ticketReport, query.PageNumber, query.PageSize);

                foreach (var ticket in pagedResult)
                {

                    var descendants = rootTicketDescendants[ticket.OriginalTicketNo.Value];
                    ticket.Backjobs = descendants.Count >= 3 ? 1
                        : descendants.Count >= 1 ? 2
                        : 3;
                    ticket.BackJobTicketNumber = string.Join(", ", descendants);

                    //var data = rootTicketTargetDates.Where(x => x.BackJobId == ticket.OriginalTicketNo).ToList();
                    //ticket.TargetDate = ticket.TargetDate + string.Join(", ", data.Where(d => d.TargetDate.HasValue).Select(d => d.TargetDate.Value.ToString("MM/dd/yyyy")));
                    //ticket.BackJobDate = ticket.BackJobDate + string.Join(", ", data.Select(d => d.CreatedAt.ToString("MM/dd/yyyy HH:mm")));

                    var descendantTargetDates = allRequestConcerns
                         .Where(x => descendants.Contains(x.Id) && x.TargetDate.HasValue)
                         .Select(d => d.TargetDate.Value.ToString("MM/dd/yyyy"))
                         .ToList();

                    var descendantCreatedDates = allRequestConcerns
                        .Where(x => descendants.Contains(x.Id))
                        .Select(d => d.CreatedAt.ToString("MM/dd/yyyy HH:mm"))
                        .ToList();

                    ticket.TargetDate = ticket.TargetDate + string.Join(", ", descendantTargetDates);
                    ticket.BackJobDate = ticket.BackJobDate + string.Join(", ", descendantCreatedDates);

                }

                return pagedResult;
            }
        }
    }
}
