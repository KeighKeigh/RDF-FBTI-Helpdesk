using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.AllTicketExport.AllTicketExport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.BackjobReport.BackJobReportHandler;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.BackJobExport
{
    public class BackJobTicketExport
    {

        public class BackJobTicketExportQuery : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? ServiceProvider { get; set; }
            public int? Channel { get; set; }
            public Guid? UserId { get; set; }
        }

        public class BackJobTicketExportResult
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


        public class Handler : IRequestHandler<BackJobTicketExportQuery, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(BackJobTicketExportQuery request, CancellationToken cancellationToken)
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

                //var rootTicketReworkDates = allRequestConcerns
                //    .Where(x => reworkIds.Contains(x.Id))
                //    .ToList();5

                var rootTicketDescendants = new Dictionary<int, List<int>>();
                //var rootTicketDates = new Dictionary<int, List<DescendantInfo>>();

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


                var ticketReport = await _context.TicketConcerns
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

                     }).ToListAsync();

                foreach (var ticket in ticketReport)
                {

                    var descendants = rootTicketDescendants[ticket.OriginalTicketNo.Value];
                    ticket.Backjobs = descendants.Count >= 3 ? 1
                        : descendants.Count >= 1 ? 2
                        : 3;
                    ticket.BackJobTicketNumber = string.Join(", ", descendants);

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


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Backjob Ticket Report");
                    var headers = new List<string>
                    {
                        "YEAR",
                        "MONTH",
                        "ORIGINAL TICKET NUMBER",
                        "BACKJOB TICKET NUMBER",
                        "CONCERN DETAILS",
                        "REASON",
                        "ISSUE HANDLER",
                        "CHANNEL",
                        "REQUESTOR NAME",
                        "COMPANY",
                        "DEPARTMENT",
                        "LOCATION",
                        "BUSINESS UNIT",
                        "UNIT",
                        "SUB-UNIT",
                        "BACK-JOB DATE",
                        "TARGET DATE",
                        "BACK JOBS",







                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= ticketReport.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);
                        row.Cell(1).Value = ticketReport[index - 1].Year;
                        row.Cell(2).Value = ticketReport[index - 1].Month;
                        row.Cell(3).Value = ticketReport[index - 1].OriginalTicketNo;
                        row.Cell(4).Value = ticketReport[index - 1].BackJobTicketNumber;
                        row.Cell(5).Value = ticketReport[index - 1].ConcernDescription;
                        row.Cell(6).Value = ticketReport[index - 1].Reason;
                        row.Cell(7).Value = ticketReport[index - 1].IssueHandler;
                        row.Cell(8).Value = ticketReport[index - 1].Channel;
                        row.Cell(9).Value = ticketReport[index - 1].RequestorName;
                        row.Cell(10).Value = $"{ticketReport[index - 1].CompanyCode} - {ticketReport[index - 1].CompanyName}";
                        row.Cell(11).Value = $"{ticketReport[index - 1].Department_Code} - {ticketReport[index - 1].Department_Name}";
                        row.Cell(12).Value = $"{ticketReport[index - 1].Location_Code} - {ticketReport[index - 1].Location_Name}";
                        row.Cell(13).Value = $"{ticketReport[index - 1].Business_Unit_Code} - {ticketReport[index - 1].Business_Unit_Name}";
                        row.Cell(14).Value = $"{ticketReport[index - 1].Unit_Code} - {ticketReport[index - 1].Unit_Name}";
                        row.Cell(15).Value = $"{ticketReport[index - 1].SubUnit_Code} - {ticketReport[index - 1].SubUnit_Name}";
                        row.Cell(16).Value = ticketReport[index - 1].BackJobDate;
                        row.Cell(17).Value = ticketReport[index - 1].TargetDate;
                        row.Cell(18).Value = ticketReport[index - 1].Backjobs;







                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"BackJobTicketExport.xlsx");

                }

                return Unit.Value;

            }
        }
    }
}
