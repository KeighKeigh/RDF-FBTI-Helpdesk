using Azure.Core;
using ClosedXML.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.SADSLAReport.SADSLAReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SADSLAExport
{
    public class SADSLATicketExport
    {

        public class SADSLATicketExportCommand : UserParams, IRequest<Unit>
        {
            public string Search { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }


        public class SADSLATicketExportResult
        {
            public int? TicketNo { get; set; }
            public string Assign { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string OpenDate { get; set; }
            public string TargetDate { get; set; }
            public string ActualDate { get; set; }
            public string Remarks { get; set; }
            public string Solution { get; set; }
        }

        public class Handler : IRequestHandler<SADSLATicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(SADSLATicketExportCommand command, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<SADSLATicketExportResult>();
                var dateToday = DateTime.Now;

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.DateApprovedAt.Value.Date >= command.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= command.Date_To.Value.Date && t.RequestConcern.ChannelId == 8)
                    .Select(o => new SADSLATicketExportResult
                    {
                        TicketNo = o.Id,
                        Assign = o.User.Fullname,
                        Category = string.Join(", ", o.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        Description = o.RequestConcern.Concern,
                        OpenDate = o.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:mm tt"),
                        TargetDate = o.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ActualDate = o.Closed_At.ToString(),
                        Remarks = o.TargetDate.Value.Date >= dateToday.Date ? "On Time" : "Delay",
                        Solution = o.RequestConcern.Resolution,

                    }).ToListAsync();


                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                .AsSplitQuery()
                .Where(x => x.IsClosing == true && x.IsActive == true)
                .Where(t => t.ClosingAt.Value.Date >= command.Date_From.Value.Date && t.ClosingAt.Value.Date <= command.Date_To.Value.Date && t.TicketConcern.RequestConcern.ChannelId == 8)
                    .Select(ct => new SADSLATicketExportResult
                    {
                        TicketNo = ct.TicketConcernId,
                        Assign = ct.TicketConcern.User.Fullname,
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        Description = ct.TicketConcern.RequestConcern.Concern,
                        OpenDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:mm tt"),
                        TargetDate = ct.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ActualDate = ct.ForClosingAt.Value.ToString("MM/dd/yyyy hh:mm tt"),
                        Remarks = ct.TicketConcern.TargetDate.Value.Date >= ct.ForClosingAt.Value.Date ? "On Time" : "Delay",
                        Solution = ct.TicketConcern.RequestConcern.Resolution,

                    }).ToListAsync();



                

                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                var results = combineTicketReports
                    .OrderBy(x => x.OpenDate)
                    .ThenBy(x => x.TicketNo)
                    .Select(r => new SADSLATicketExportResult
                    {

                        TicketNo = r.TicketNo,
                        Assign = r.Assign,
                        Category = r.Category,
                        Description = r.Description,
                        OpenDate = r.OpenDate,
                        TargetDate = r.TargetDate,
                        ActualDate = r.ActualDate,
                        Remarks = r.Remarks,
                        Solution = r.Solution,

                    }).ToList();

                if (!string.IsNullOrEmpty(command.Remarks))
                {
                    switch (command.Remarks)
                    {
                        case TicketingConString.OnTime:
                            results = results
                                .Where(x => x.Remarks == "On Time")
                            .ToList();
                            break;

                        case TicketingConString.Delay:
                            results = results
                                .Where(x => x.Remarks == "Delay")
                                .ToList();
                            break;

                        default:
                            return Unit.Value;

                    }
                }


                if (!string.IsNullOrEmpty(command.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(command.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketNo.ToString().ToLower().Contains(command.Search)
                        || x.Assign.ToLower().Contains(command.Search)
                        || x.Remarks.ToLower().Contains(command.Search)
                        || x.Solution.ToLower().Contains(command.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();


                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"SLA Report");
                    var headers = new List<string>
                    {
                        "Ticket Number",
                        "Assign",
                        "Category",
                        "Description",
                        "Open Date",
                        "Target",
                        "Actual",
                        "Remarks",
                        "Solution"

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
                    for (var index = 1; index <= results.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = results[index - 1].TicketNo;
                        row.Cell(2).Value = results[index - 1].Assign;
                        row.Cell(3).Value = results[index - 1].Category;
                        row.Cell(4).Value = results[index - 1].Description;
                        row.Cell(5).Value = results[index - 1].OpenDate;
                        row.Cell(6).Value = results[index - 1].TargetDate;
                        row.Cell(7).Value = results[index - 1].ActualDate;
                        row.Cell(8).Value = results[index - 1].Remarks;
                        row.Cell(9).Value = results[index - 1].Solution;
                    

                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"SADSLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");
                }

                return Unit.Value;
            }
        }
    }
}
