using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport
{
    public partial class AllTicketReports
    {

        public class Handler : IRequestHandler<AllTicketReportsQuery, PagedList<AllTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<AllTicketReportsResult>> Handle(AllTicketReportsQuery request, CancellationToken cancellationToken)
            {

                var combineTicketReports = new List<AllTicketReportsResult>();

                //var cutoffTime = TimeSpan.FromHours(16);

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                    .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.CreatedAt.Date >= request.Date_From.Value.Date && t.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(o => new AllTicketReportsResult
                    {

                        TicketConcernId = o.Id.ToString(),

                        Request_Type = o.RequestConcern.RequestType,
                        BackJobId = o.RequestConcern.BackJobId.ToString(),
                        Requestor_Name = o.RequestorByUser.Fullname,
                        Company_Code = o.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = o.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnit_Code = o.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = o.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Code = o.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = o.RequestConcern.OneChargingMIS.department_name,
                        Unit_Code = o.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = o.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Code = o.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = o.RequestConcern.OneChargingMIS.sub_unit_name,
                        Location_Code = o.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = o.RequestConcern.OneChargingMIS.location_name,
                        Personnel_Unit = o.User.UnitId,
                        Personnel_Id = o.UserId,
                        Personnel = o.User.Fullname,
                        Concerns = o.RequestConcern.Concern,
                        Channel_Name = o.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", o.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)), 
                        TicketSubCategoryDescriptions = string.Join(", ", o.RequestConcern.TicketSubCategories
                           .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = o.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy"),
                        Contact_Number = o.RequestConcern.ContactNumber,
                        Notes = o.RequestConcern.Notes,
                        Transaction_Date = o.CreatedAt.ToString("MM/dd/yyyy hh:tt:mm"),
                        Target_Date = o.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Ticket_Status = "Open",
                        Remarks = o.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(o.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = o.RequestConcern.ChannelId.Value,
                        StartDate = o.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ServiceProvider = o.RequestConcern.ServiceProviderId.Value,
                        AssignTo = o.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = o.RequestConcern.ServiceProvider.ServiceProviderName,
                        Resolution = o.RequestConcern.Resolution,
                        
                        
                        
                        
                    }).ToListAsync();

                var transferTicketQuery = await _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsTransfer == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId.ToString(),
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.TicketConcern.RequestConcern.BackJobId.ToString(),
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Unit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                        Location_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TransferToUser.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.TransferAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        Target_Date = ct.Current_Target_Date.Value.ToString("MM/dd/yyyy"),
                        Ticket_Status = "Transfer",
                        Remarks = ct.TransferRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TransferToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName
                    }).ToListAsync();

                var onHoldTicketQuery = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsHold == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId.ToString(),
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType,
                        BackJobId = ct.TicketConcern.RequestConcern.BackJobId.ToString(),
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Unit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                        Location_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TicketConcern.User.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.CreatedAt.ToString("MM/dd/yyyy hh:tt:mm"),
                        Target_Date = ct.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Ticket_Status = "On-Hold",
                        Remarks = ct.OnHoldRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName

                    }).ToListAsync();

                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsActive == true && x.IsClosing == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date )
                    .Select(ct => new AllTicketReportsResult
                    {
                        TicketConcernId = ct.TicketConcernId.ToString(),
                        Request_Type = ct.TicketConcern.RequestConcern.RequestType == null ? "" : ct.TicketConcern.RequestConcern.RequestType,
                         BackJobId = ct.TicketConcern.RequestConcern.BackJobId.ToString() == null ? "" : ct.TicketConcern.RequestConcern.BackJobId.ToString(),
                        Requestor_Name = ct.TicketConcern.RequestorByUser.Fullname == null ? "" : ct.TicketConcern.RequestorByUser.Fullname,
                        Company_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.company_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.company_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        Department_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Unit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnit_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                        Location_Code = ct.TicketConcern.RequestConcern.OneChargingMIS.location_code == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = ct.TicketConcern.RequestConcern.OneChargingMIS.location_name == null ? "" : ct.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        Personnel_Unit = ct.TicketConcern.User.UnitId,
                        Personnel_Id = ct.TicketConcern.UserId,
                        Personnel = ct.TicketConcern.User.Fullname,
                        Concerns = ct.TicketConcern.RequestConcern.Concern,
                        Channel_Name = ct.TicketConcern.RequestConcern.Channel.ChannelName,
                        TicketCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        TicketSubCategoryDescriptions = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories
                               .Select(x => x.SubCategory.SubCategoryDescription)),
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.Notes,
                        Transaction_Date = ct.ClosingAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        Target_Date = ct.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Ticket_Status = "Closed",
                        Remarks = ct.ClosingRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, ct.ClosingAt.Value.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ClosedDate = ct.TicketConcern.Closed_At.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ForClosedDate = ct.ForClosingAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ClosingStatus = ct.TicketConcern.TargetDate.Value.Date >= ct.TicketConcern.Closed_At.Value.Date ? "On-Time"
                        : "Delayed",
                        Resolution = ct.TicketConcern.RequestConcern.Resolution,
                        Technicians = string.Join(", ", ct.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname)),
                        CategoryConcern = ct.TicketConcern.RequestConcern.CategoryConcernName,

                    }).ToListAsync(); 


                //var closingTicketTechnicianQuery = await _context.TicketTechnicians
                //    .AsNoTrackingWithIdentityResolution()
                //    .Include(ct => ct.ClosingTicket)
                //    .ThenInclude(ct => ct.TicketConcern)
                //    .ThenInclude(ct => ct.RequestConcern)
                //    .Include(ct => ct.TechnicianByUser)
                //    .AsSplitQuery()
                //    .Where(ct => ct.ClosingTicket.IsClosing == true && ct.ClosingTicket.IsActive == true)
                //    .Where(t => t.ClosingTicket.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingTicket.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                //    .Select(ct => new AllTicketReportsResult
                //    {
                //        TicketConcernId = ct.ClosingTicket.TicketConcernId.ToString(),
                //        Request_Type = ct.ClosingTicket.TicketConcern.RequestConcern.RequestType,
                //        BackJobId = ct.ClosingTicket.TicketConcern.RequestConcern.BackJobId.ToString(),
                //        Requestor_Name = ct.ClosingTicket.TicketConcern.RequestorByUser.Fullname,
                //        Company_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                //        Company_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                //        BusinessUnit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                //        BusinessUnit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                //        Department_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                //        Department_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                //        Unit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                //        Unit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                //        SubUnit_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                //        SubUnit_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                //        Location_Code = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                //        Location_Name = ct.ClosingTicket.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                //        Personnel_Unit = ct.TechnicianByUser.UnitId,
                //        Personnel_Id = ct.TechnicianBy,
                //        Personnel = ct.TechnicianByUser.Fullname,
                //        Concerns = ct.ClosingTicket.TicketConcern.RequestConcern.Concern,
                //        Channel_Name = ct.ClosingTicket.TicketConcern.RequestConcern.Channel.ChannelName,
                //        TicketCategoryDescriptions = string.Join(", ",ct.ClosingTicket.TicketConcern.RequestConcern.TicketCategories
                //           .Select(x => x.Category.CategoryDescription)),
                //        TicketSubCategoryDescriptions = string.Join(", ", ct.ClosingTicket.TicketConcern.RequestConcern.TicketSubCategories
                //            .Select(x => x.SubCategory.SubCategoryDescription)), 
                //        Date_Needed = ct.ClosingTicket.TicketConcern.RequestConcern.DateNeeded,
                //        Contact_Number = ct.ClosingTicket.TicketConcern.RequestConcern.ContactNumber,
                //        Notes = ct.ClosingTicket.Notes,
                //        Transaction_Date = ct.ClosingTicket.ClosingAt.Value.Date,
                //        Target_Date = ct.ClosingTicket.TicketConcern.TargetDate.Value.Date,
                //        Ticket_Status = "Closed",
                //        Remarks = ct.ClosingTicket.ClosingRemarks,
                //        Aging_Days = EF.Functions.DateDiffDay(ct.ClosingTicket.TicketConcern.DateApprovedAt.Value.Date, ct.ClosingTicket.ClosingAt.Value.Date),
                //        ChannelId = ct.ClosingTicket.TicketConcern.RequestConcern.ChannelId.Value,
                //        StartDate = ct.ClosingTicket.TicketConcern.DateApprovedAt,
                //        ClosedDate = ct.ClosingTicket.TicketConcern.Closed_At,
                //        ServiceProvider = ct.ClosingTicket.TicketConcern.RequestConcern.ServiceProviderId.Value,
                //        AssignTo = ct.ClosingTicket.TicketConcern.RequestConcern.AssignToUser.Fullname,
                //        ClosingStatus = ct.ClosingTicket.TicketConcern.TargetDate.Value.Date >= ct.ClosingTicket.TicketConcern.Closed_At.Value.Date ? "On-Time"
                //        : "Delayed",
                //        Resolution = ct.ClosingTicket.TicketConcern.RequestConcern.Resolution,
                //    }).ToListAsync();

                if (request.ServiceProvider is not null)
                {
                    openTicketQuery = openTicketQuery
                           .Where(x => x.ServiceProvider == request.ServiceProvider)
                           .ToList();

                    transferTicketQuery = transferTicketQuery
                       .Where(x => x.ServiceProvider == request.ServiceProvider)
                       .ToList();

                    onHoldTicketQuery = onHoldTicketQuery
                       .Where(x => x.ServiceProvider == request.ServiceProvider)
                       .ToList();

                    closingTicketQuery = closingTicketQuery
                       .Where(x => x.ServiceProvider == request.ServiceProvider)
                       .ToList();

                    //closingTicketTechnicianQuery = closingTicketTechnicianQuery
                    //   .Where(x => x.ServiceProvider == request.ServiceProvider)
                    //   .ToList();

                    if (request.Channel is not null)
                    {
                        openTicketQuery = openTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        transferTicketQuery = transferTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        onHoldTicketQuery = onHoldTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        closingTicketQuery = closingTicketQuery
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        //closingTicketTechnicianQuery = closingTicketTechnicianQuery
                        //   .Where(x => x.ChannelId == request.Channel)
                        //   .ToList();



                        if (request.UserId is not null)
                        {
                            openTicketQuery = openTicketQuery
                                .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();

                            transferTicketQuery = transferTicketQuery
                                    .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();

                            onHoldTicketQuery = onHoldTicketQuery
                                    .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();

                            closingTicketQuery = closingTicketQuery
                                    .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();

                            //closingTicketTechnicianQuery = closingTicketTechnicianQuery
                            //        .Where(x => x.Personnel_Id == request.UserId)
                            //    .ToList();
                        }
                    }
                }


                //if (!string.IsNullOrEmpty(request.Search))
                //{
                //    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                //    openTicketQuery = openTicketQuery
                //    .Where(x => x.TicketConcernId.Contains(request.Search)
                //        || x.Personnel.ToLower().Contains(request.Search)
                //        || x.Request_Type.ToLower().Contains(request.Search)
                //        || x.BackJobId.Contains(request.Search)
                //        || x.Requestor_Name.ToLower().Contains(request.Search)
                //        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                //        || x.AssignTo.ToLower().Contains(request.Search)).ToList();

                //    transferTicketQuery = transferTicketQuery
                //    .Where(x => x.TicketConcernId.Contains(request.Search)
                //        || x.Personnel.ToLower().Contains(request.Search)
                //        || x.Request_Type.ToLower().Contains(request.Search)
                //        || x.BackJobId.Contains(request.Search)
                //        || x.Requestor_Name.ToLower().Contains(request.Search)
                //        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                //        || x.AssignTo.ToLower().Contains(request.Search)).ToList();

                //    onHoldTicketQuery = onHoldTicketQuery
                //   .Where(x => x.TicketConcernId.Contains(request.Search)
                //        || x.Personnel.ToLower().Contains(request.Search)
                //        || x.Request_Type.ToLower().Contains(request.Search)
                //        || x.BackJobId.Contains(request.Search)
                //        || x.Requestor_Name.ToLower().Contains(request.Search)
                //        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                //        || x.AssignTo.ToLower().Contains(request.Search)).ToList();
                //    closingTicketQuery = closingTicketQuery
                //   .Where(x => x.TicketConcernId.Contains(request.Search)
                //        || x.Personnel.ToLower().Contains(request.Search)
                //        || x.Request_Type.ToLower().Contains(request.Search)
                //        || x.BackJobId.Contains(request.Search)
                //        || x.Requestor_Name.ToLower().Contains(request.Search)
                //        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                //        || x.AssignTo.ToLower().Contains(request.Search)).ToList();

                //    //closingTicketTechnicianQuery = closingTicketTechnicianQuery
                //    //.Where(x => x.TicketConcernId.Contains(request.Search)
                //    //    || x.Personnel.ToLower().Contains(request.Search)
                //    //    || x.Request_Type.ToLower().Contains(request.Search)
                //    //    || x.BackJobId.Contains(request.Search)
                //    //    || x.Requestor_Name.ToLower().Contains(request.Search)
                //    //    || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                //    //    || x.AssignTo.ToLower().Contains(request.Search)).ToList();
                //}

                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }
                foreach (var list in transferTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in onHoldTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                //foreach (var list in closingTicketTechnicianQuery)
                //{
                //    combineTicketReports.Add(list);
                //}
                
                var results =  combineTicketReports
                    .OrderBy(x => x.Transaction_Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(r => new AllTicketReportsResult
                    {
                        TicketConcernId = r.TicketConcernId,
                        Request_Type = r.Request_Type,
                        BackJobId = r.BackJobId,
                        Requestor_Name = r.Requestor_Name,
                        Company_Code = r.Company_Code,
                        Company_Name = r.Company_Name,
                        BusinessUnit_Code = r.BusinessUnit_Code,
                        BusinessUnit_Name = r.BusinessUnit_Name,
                        Department_Code = r.Department_Code,
                        Department_Name = r.Department_Name,
                        Unit_Code = r.Unit_Code,
                        Unit_Name = r.Unit_Name,
                        SubUnit_Code = r.SubUnit_Code,
                        SubUnit_Name = r.SubUnit_Name,
                        Location_Code = r.Location_Code,
                        Location_Name = r.Location_Name,
                        Personnel = r.Personnel,
                        Concerns = r.Concerns,
                        Channel_Name = r.Channel_Name,
                        TicketCategoryDescriptions = r.TicketCategoryDescriptions,
                        TicketSubCategoryDescriptions = r.TicketSubCategoryDescriptions,
                        Date_Needed = r.Date_Needed,    
                        Contact_Number = r.Contact_Number,
                        Notes = r.Notes,
                        Transaction_Date = r.Transaction_Date,
                        Target_Date = r.Target_Date,
                        Ticket_Status = r.Ticket_Status,
                        Remarks = r.Remarks,
                        Aging_Days = r.Aging_Days,
                        ChannelId = r.ChannelId,
                        StartDate = r.StartDate,
                        ClosedDate = r.ClosedDate,
                        AssignTo = r.AssignTo,
                        ServiceProvider = r.ServiceProvider,
                        ServiceProviderName = r.ServiceProviderName,
                        ClosingStatus = r.ClosingStatus,
                        Resolution = r.Resolution,
                        Technicians = r.Technicians,
                        CategoryConcern = r.CategoryConcern,
                        ForClosedDate = r.ForClosedDate,
                       

                    }).ToList();


                if (!string.IsNullOrEmpty(request.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketConcernId.Contains(request.Search)
                        || x.Personnel.ToLower().Contains(request.Search)
                        || x.Requestor_Name.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                        || x.AssignTo.ToLower().Contains(request.Search)).ToList();

                }

                return PagedList<AllTicketReportsResult>.Create(results.AsQueryable(), request.PageNumber, request.PageSize);


            }
        }

    }
}
