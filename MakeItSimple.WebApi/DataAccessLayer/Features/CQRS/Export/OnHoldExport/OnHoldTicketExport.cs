using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Export.OpenExport.OpenTicketExport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport.OnHoldTicketReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public partial class OnHoldTicketExport
    {

        public class Handler : IRequestHandler<OnHoldTicketExportQuery, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(OnHoldTicketExportQuery request, CancellationToken cancellationToken)
            {

                var query = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery()
                    .Where(r => r.CreatedAt.Date >= request.Date_From.Value.Date && r.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(r => new OnHoldTicketExportResult
                    {
                        UserId = r.AddedBy,
                        Unit = r.AddedByUser.UnitId,
                        TicketConcernId = r.TicketConcernId.Value,
                        Concerns = r.TicketConcern.RequestConcern.Concern,
                        Reason = r.Reason,
                        Added_By = r.AddedByUser.Fullname,
                        Created_At = r.CreatedAt,
                        IsHold = r.IsHold,
                        Resume_At = r.ResumeAt,
                        ApprovedAt = r.ApprovedAt,
                        ApprovedBy = r.ApprovedBy,
                        ServiceProviderId = r.TicketConcern.RequestConcern.ServiceProviderId,
                        ServiceProviderName = r.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ChannelId = r.TicketConcern.RequestConcern.ChannelId,
                        ChannelName = r.TicketConcern.RequestConcern.Channel.ChannelName,
                        Year = r.TicketConcern.TargetDate.Value.Year,
                        Month = r.TicketConcern.TargetDate.Value.Month


                    }).ToListAsync();

                if (request.ServiceProvider is not null)
                {
                    query = query.Where(x => x.ServiceProviderId == request.ServiceProvider).ToList();

                    if (request.Channel is not null)
                    {
                        query = query.Where(x => x.ChannelId == request.Channel).ToList();

                        if (request.UserId is not null)
                        {
                            query = query.Where(x => x.UserId == request.UserId).ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    query = query
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Added_By.Contains(request.Search)).ToList();
                }
                var finalQuery = query
                    .OrderBy(x => x.Created_At.Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(r => new OnHoldTicketExportResult
                    {
                        UserId = r.UserId,
                        Unit = r.Unit,
                        TicketConcernId = r.TicketConcernId,
                        Concerns = r.Concerns,
                        Reason = r.Reason,
                        Added_By = r.Added_By,
                        Created_At = r.Created_At,
                        IsHold = r.IsHold,
                        Resume_At = r.Resume_At,
                        ApprovedAt = r.ApprovedAt,
                        ApprovedBy = r.ApprovedBy,
                        ServiceProviderId = r.ServiceProviderId,
                        ServiceProviderName = r.ServiceProviderName,
                        ChannelId = r.ChannelId,
                        ChannelName = r.ChannelName,
                        Year = r.Year,
                        Month = r.Month,
                    }).ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"OnHold Ticket Report");
                    var headers = new List<string>
                    {
                        "YEAR",
                        "MONTH",
                        "TICKET NUMBER",
                        "CONCERN DETAILS",
                        "Reason",
                        "Hold By",
                        "Channel",
                        "Hold Date",
                        "Resume Date",
                        "Approved Date",
                        "Approved By",
                        
                        
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

                    for (var index = 1; index <= finalQuery.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = finalQuery[index - 1].Year;
                        row.Cell(2).Value = finalQuery[index - 1].Month;
                        row.Cell(3).Value = finalQuery[index - 1].TicketConcernId;
                        row.Cell(4).Value = finalQuery[index - 1].Concerns;
                        row.Cell(5).Value = finalQuery[index - 1].Reason;
                        row.Cell(6).Value = finalQuery[index - 1].Added_By;
                        row.Cell(7).Value = finalQuery[index - 1].ChannelName;
                        row.Cell(8).Value = finalQuery[index - 1].Created_At;
                        row.Cell(9).Value = finalQuery[index - 1].Resume_At;
                        row.Cell(10).Value = finalQuery[index - 1].ApprovedAt;
                        row.Cell(11).Value = finalQuery[index - 1].ApprovedBy;
                        
                    }


                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"OnHoldTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }

    }
}
