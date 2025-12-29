using ClosedXML.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {

        public class Handler : IRequestHandler<ClosingTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ClosingTicketExportCommand request, CancellationToken cancellationToken)
            {

                var requestConcernList = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => x.IsActive && x.BackJobId != null).ToListAsync();

                var backJobIds = requestConcernList.Select(x => x.BackJobId.Value).Distinct().ToList();

                var requestConcernWithBackjob = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => backJobIds.Contains(x.Id)).ToListAsync();

                var closing =  await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsActive == true && x.IsClosing == true)
                    .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new ClosingTicketExportResult
                    {
                        Year = x.TicketConcern.TargetDate.Value.Year.ToString(),
                        Month = x.TicketConcern.TargetDate.Value.Month.ToString(),
                        IssueHandler = x.TicketConcern.User.Fullname,
                        Ticket_Number = x.TicketConcernId,
                        Description = x.TicketConcern.RequestConcern.Concern,
                        Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ClosedDate =  x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        Varience = x.ClosingAt.Value.Date > x.TicketConcern.TargetDate.Value.Date  ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) : 0,
                        Efficeincy = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? "100 %" : "50 %",
                        Status = TicketingConString.Closed,
                        ClosingStatus = x.ClosingAt.Value.Date  <= x.TicketConcern.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = string.Join(", ", x.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", x.TicketConcern.RequestConcern.TicketSubCategories
                          .Select(x => x.SubCategory.SubCategoryDescription)),
                        Aging_Days = EF.Functions.DateDiffDay(x.TicketConcern.DateApprovedAt.Value.Date, x.ClosingAt.Value.Date),
                        Start_Date = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy"),
                        ForClosedDate = x.ForClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt") ?? "",
                        ServiceProvider = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        ServiceProviderName = x.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ChannelName = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        UserId = x.TicketConcern.AssignTo,
                        CreatedAt = x.TicketConcern.CreatedAt.ToString("MM/dd/yyyy hh:mm:tt"),
                        ConfirmedAt = x.TicketConcern.RequestConcern.Confirm_At.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        OpenDate = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        //Technician1 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(0).Take(1).FirstOrDefault(),
                        //Technician2 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(1).Take(1).FirstOrDefault(),
                        //Technician3 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(2).Take(1).FirstOrDefault(),
                        Technicians = string.Join(", ", x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname)),
                        Company_Code = x.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = x.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code, 
                        BusinessUnit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name, 
                        Department_Code = x.TicketConcern.RequestConcern.OneChargingMIS.department_code, 
                        Department_Name = x.TicketConcern.RequestConcern.OneChargingMIS.department_name, 
                        Unit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code, 
                        Unit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name, 
                        SubUnit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code, 
                        SubUnit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name, 
                        Location_Code = x.TicketConcern.RequestConcern.OneChargingMIS.location_code, 
                        Location_Name = x.TicketConcern.RequestConcern.OneChargingMIS.location_name, 
                        Requestor = x.TicketConcern.RequestorByUser.Fullname,
                        CategoryConcern = x.CategoryConcernName,
                        Contractor = x.Contractor,
                        Resolution = x.TicketConcern.RequestConcern.Resolution,
                        DatePicked = x.TicketConcern.RequestConcern.DatePicked.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        RequestType = x.TicketConcern.RequestConcern.RequestType,
                        Rating = EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 31 ? 1
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 15 ? 2
                        : 3,
                        SLAPercentage = EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 31 ? "95%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 24 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 30 ? "96%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 16 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 23 ? "97%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 11 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 15 ? "98%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 6 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 10 ? "99%"
                        : "100%",
                        //Department = x.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Notes =x.Notes

                    }).ToListAsync();


                if (request.ServiceProvider is not null)
                {
                    closing = closing.Where(x => x.ServiceProvider == request.ServiceProvider).ToList();

                    if (request.Channel is not null)
                    {
                        closing = closing.Where(x => x.ChannelId == request.Channel).ToList();

                        if (request.UserId is not null)
                        {
                            closing = closing.Where(x => x.UserId == request.UserId).ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Remarks))
                {
                    switch (request.Remarks)
                    {
                        case TicketingConString.OnTime:
                            closing = closing
                                .Where(x => x.ClosedDate != null && x.Target_Date_DateTime.Value.Date > x.Actual_Date_DateTime.Value.Date)
                                .ToList();
                            break;

                        case TicketingConString.Delay:
                            closing = closing
                                .Where(x => x.ClosedDate != null && x.Target_Date_DateTime.Value.Date < x.Actual_Date_DateTime.Value.Date)
                                .ToList();
                            break;

                        default:
                            return Unit.Value;

                    }
                }

                foreach (var ticket in closing)
                {
                    ticket.Backjobs = requestConcernWithBackjob.Count(x => x.Id == ticket.Ticket_Number) >= 3 ? 1
                        : requestConcernWithBackjob.Count(x => x.Id == ticket.Ticket_Number) >= 1 ? 2
                        : 3;
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    closing = closing
                        .Where(x => x.Ticket_Number.ToString().Contains(request.Search)
                        || x.IssueHandler.Contains(request.Search))
                        .ToList();
                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Closing Ticket Report");
                    var headers = new List<string>
                    {
                        "YEAR",
                        "MONTH",
                        "TICKET NUMBER",
                        "CHANNEL",
                        "ISSUE HANDLER",
                        "REQUESTOR",
                        "COMPANY",
                        "DEPARTMENT",
                        "LOCATION",
                        "BUSINESS UNIT",
                        "UNIT",
                        "SUB-UNIT",
                        "CONCERN DETAILS",
                        "CATEGORY",
                        "SUB-CATEGORY",
                        "CONCERN CATEGORY",
                        "CONTRACTOR",
                        "RESOLUTION",
                        "TECHNICIAN",
                        "DATE REQUESTED",
                        "DATE PICKED",
                        "TARGET DATE",
                        "CLOSED DATE",
                        "APPROVER CLOSE DATE",
                        "REQUEST TYPE",
                        "RATING",
                        "COMPLETED WITHIN SLA",
                        "SLA%",
                        "BACK JOBS"


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
                    for (var index = 1; index <= closing.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = closing[index - 1].Year;
                        row.Cell(2).Value = closing[index - 1].Month;
                        row.Cell(3).Value = closing[index - 1].Ticket_Number;
                        row.Cell(4).Value = closing[index - 1].ChannelName;
                        row.Cell(5).Value = closing[index - 1].IssueHandler;
                        row.Cell(6).Value = closing[index - 1].Requestor;
                        row.Cell(7).Value = $"{closing[index - 1].Company_Code} - {closing[index - 1].Company_Name}";
                        row.Cell(8).Value = $"{closing[index - 1].Department_Code} - {closing[index - 1].Department_Name}";
                        row.Cell(9).Value = $"{closing[index - 1].Location_Code} - {closing[index - 1].Location_Name}";
                        row.Cell(10).Value = $"{closing[index - 1].BusinessUnit_Code} - {closing[index - 1].BusinessUnit_Name}";
                        row.Cell(11).Value = $"{closing[index - 1].Unit_Code} - {closing[index - 1].Unit_Name}";
                        row.Cell(12).Value = $"{closing[index - 1].SubUnit_Code} - {closing[index - 1].SubUnit_Name}";
                        row.Cell(13).Value = closing[index - 1].Description;
                        row.Cell(14).Value = closing[index - 1].Category;
                        row.Cell(15).Value = closing[index - 1].SubCategory;
                        row.Cell(16).Value = closing[index - 1].CategoryConcern;
                        row.Cell(17).Value = closing[index - 1].Contractor;
                        row.Cell(18).Value = closing[index - 1].Resolution;
                        row.Cell(19).Value = closing[index - 1].Technicians;
                        row.Cell(20).Value = closing[index - 1].CreatedAt;
                        row.Cell(21).Value = closing[index - 1].DatePicked;
                        row.Cell(22).Value = closing[index - 1].Target_Date;
                        row.Cell(23).Value = closing[index - 1].ForClosedDate;
                        row.Cell(24).Value = closing[index - 1].ClosedDate;
                        row.Cell(25).Value = closing[index - 1].RequestType;
                        row.Cell(26).Value = closing[index - 1].ClosingStatus;
                        row.Cell(27).Value = closing[index - 1].Rating;
                        row.Cell(28).Value = closing[index - 1].SLAPercentage;
                        row.Cell(29).Value = closing[index - 1].Backjobs;


                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ClosingTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }
    }
}
