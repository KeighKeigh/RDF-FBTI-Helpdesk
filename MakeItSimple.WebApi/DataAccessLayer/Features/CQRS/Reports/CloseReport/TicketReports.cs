using Humanizer;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport
{
    public partial class TicketReports
    {

        public class Handler : IRequestHandler<TicketReportsQuery, PagedList<Reports>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<Reports>> Handle(TicketReportsQuery request, CancellationToken cancellationToken)
            {

                var requestConcernList = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => x.IsActive && x.BackJobId != null).ToListAsync();

                var backJobIds = requestConcernList.Select(x => x.BackJobId.Value).Distinct().ToList();

                var requestConcernWithBackjob = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => backJobIds.Contains(x.Id)).ToListAsync();




                var closingTicket =  _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsActive == true && x.IsClosing == true)
                    .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new Reports
                    {
                        Year = x.TicketConcern.TargetDate.Value.Year,
                        Month = x.TicketConcern.TargetDate.Value.Month,
                        Personnel = x.TicketConcern.User.Fullname,
                        Ticket_Number = x.TicketConcernId,
                        Description = x.TicketConcern.RequestConcern.Concern,
                        Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Actual =  x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        Varience = x.ClosingAt.Value.Date > x.TicketConcern.TargetDate.Value.Date  ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) : 0,
                        Efficeincy = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? "100 %" : "50 %",
                        Status = TicketingConString.Closed,
                        Remarks = x.ClosingAt.Value.Date  <= x.TicketConcern.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = string.Join(", ", x.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", x.TicketConcern.RequestConcern.TicketSubCategories
                          .Select(x => x.SubCategory.SubCategoryDescription)),
                        Aging_Day = EF.Functions.DateDiffDay(x.TicketConcern.DateApprovedAt.Value.Date, x.ClosingAt.Value.Date),
                        StartDate = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy"),
                        ClosedDate = x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        ForClosedDate = x.ForClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt") ?? "",
                        ServiceProviderId = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = x.TicketConcern.AssignTo,
                        ChannelName = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        Technicians = string.Join(", ", x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname)),
                        //Technician1 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(0).Take(1).FirstOrDefault(),
                        //Technician2 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(1).Take(1).FirstOrDefault(),
                        //Technician3 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(2).Take(1).FirstOrDefault(),
                        IsStore = x.TicketConcern.RequestConcern.User.IsStore,
                        Requestor = x.TicketConcern.RequestorByUser.Fullname,
                        CategoryConcern = x.CategoryConcernName,
                        Company_Code = x.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = x.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        Department_Code = x.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = x.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Location_Code = x.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = x.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        BusinessUnit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        Unit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Code = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                        DateRequested = x.TicketConcern.RequestConcern.CreatedAt.ToString("MM/dd/yyyy hh:mm:tt"),
                        Notes =x.Notes,
                        Rating =  EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 31 ? 1
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 15 ? 2
                        : 3,
                        SLAPercentage = EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 31 ? "95%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 24 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 30 ? "96%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 16 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 23 ? "97%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 11 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 15 ? "98%"
                        : EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) >= 6 && EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) <= 10 ? "99%"
                        : "100%",
                        Resolution = x.TicketConcern.RequestConcern.Resolution,
                        Contractor = x.Contractor,
                        DatePicked = x.TicketConcern.RequestConcern.DatePicked,
                        RequestType = x.TicketConcern.RequestConcern.RequestType,
                    });


                if (request.ServiceProvider is not null)
                {
                    closingTicket = closingTicket.Where(x => x.ServiceProviderId == request.ServiceProvider);

                    if (request.Channel is not null)
                    {
                        closingTicket = closingTicket.Where(x => x.ChannelId == request.Channel);

                        if (request.UserId is not null)
                        {
                            closingTicket = closingTicket.Where(x => x.AssignTo == request.UserId);
                        }
                    }
                }

                foreach (var ticket in closingTicket)
                {
                    ticket.Backjobs = requestConcernWithBackjob.Count(x => x.Id == ticket.Ticket_Number) >= 3 ? 1
                        : requestConcernWithBackjob.Count(x => x.Id == ticket.Ticket_Number) >= 1 ? 2
                        : 3;
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    closingTicket = closingTicket
                        .Where(x => x.Ticket_Number.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Description.Contains(request.Search)
                        || x.ChannelName.Contains(request.Search));
                }


                var results = closingTicket.Select(x => new Reports
                {
                    Year = x.Year,
                    Month = x.Month,
                    //Start_Date = $"{x.Month}-01-{x.Year}",
                    //End_Date = $"{x.Month}-{DateTime.DaysInMonth(x.Year, x.Month)}-{x.Year}",
                    Personnel = x.Personnel,
                    Ticket_Number = x.Ticket_Number,
                    Description = x.Description,
                    Target_Date = x.Target_Date,
                    Actual = x.Actual,
                    Varience = x.Varience,
                    Efficeincy = x.Efficeincy,
                    Status = x.Status,
                    Remarks = x.Remarks,
                    Category = x.Category,
                    SubCategory = x.SubCategory,
                    Aging_Day = x.Aging_Day,
                    StartDate = x.StartDate,
                    ClosedDate = x.ClosedDate,
                    Technicians = x.Technicians,
                    //Technician1 = x.Technician1,
                    //Technician2 = x.Technician2,
                    //Technician3 = x.Technician3,
                    AssignTo = x.AssignTo,
                    IsStore = x.IsStore,
                    CategoryConcern = x.CategoryConcern,
                    ForClosedDate = x.ForClosedDate,
                    Notes = x.Notes,
                    Company_Code = x.Company_Code,
                    Company_Name = x.Company_Name,
                    Department_Code = x.Department_Code,
                    Department_Name = x.Department_Name,   
                    Location_Code = x.Location_Code,
                    Location_Name = x.Location_Name,
                    BusinessUnit_Code = x.BusinessUnit_Code,
                    BusinessUnit_Name = x.BusinessUnit_Name,
                    Unit_Code = x.Unit_Code,
                    Unit_Name = x.Unit_Name,
                    SubUnit_Code = x.SubUnit_Code,
                    SubUnit_Name = x.SubUnit_Name,
                    DateRequested = x.DateRequested,
                    Rating = x.Rating,
                    SLAPercentage = x.SLAPercentage,
                    Resolution = x.Resolution,
                    Contractor = x.Contractor,
                    Backjobs = x.Backjobs,
                    ChannelName  = x.ChannelName,
                    Requestor = x.Requestor,
                    DatePicked = x.DatePicked,
                    RequestType = x.RequestType



                }).OrderBy(x => x.Ticket_Number); 

                return await PagedList<Reports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }

    }
}
