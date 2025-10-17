using ClosedXML.Excel;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport.AllTicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.AllTicketExport
{
    public class AllTicketExport
    {

        public class AllTicketExportCommand : IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Channel { get; set; }
            public int? ServiceProvider { get; set; }
            public Guid? UserId { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }
        public record class AllTicketExportResult
        {
            public string TicketConcernId { get; set; }
            public string Request_Type { get; set; }
            public string BackJobId { get; set; }
            public string Requestor_Name { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public int? Personnel_Unit { get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public string Concerns { get; set; }
            public string Channel_Name { get; set; }
            public string TicketCategoryDescriptions { get; set; }
            public string TicketSubCategoryDescriptions { get; set; }
            public string Date_Needed { get; set; }
            public string Contact_Number { get; set; }
            public string Notes { get; set; }
            public string Transaction_Date { get; set; }
            public string Target_Date { get; set; }
            public string Ticket_Status { get; set; }
            public string Remarks { get; set; }
            public int? Aging_Days { get; set; }
            public int? ChannelId { get; set; }
            public int? ServiceProvider { get; set; }
            public string ServiceProviderName { get; set; }
            public string StartDate { get; set; }
            public string ForClosingDate { get; set; }
            public string ClosedDate { get; set; }
            public string ForClosedAt { get; set; }
            public string AssignTo { get; set; }
            public string ClosingStatus { get; set; }
            public string Technician1 { get; set; }
            public string Technician2 { get; set; }
            public string Technician3 { get; set; }
            public string Resolution { get; set; }
            public string CreatedTime { get; set; }
            public string CompletedTime { get; set; }
            public string Severity { get; set; }
            public string DateStarted { get; set; }
            public int? RequestConcernId { get; set; }
            


        }

        public class Handler : IRequestHandler<AllTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AllTicketExportCommand request, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<AllTicketExportResult>();


                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                    .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.CreatedAt.Date >= request.Date_From.Value.Date && t.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(o => new AllTicketExportResult
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
                        Date_Needed = o.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy HH:mm"),
                        Contact_Number = o.RequestConcern.ContactNumber,
                        Notes = o.RequestConcern.Notes,
                        Transaction_Date = o.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        Target_Date = o.TargetDate.Value.Date.ToString("MM/dd/yyyy "),
                        Ticket_Status = "Open",
                        Remarks = o.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(o.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = o.RequestConcern.ChannelId.Value,
                        StartDate = o.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ServiceProvider = o.RequestConcern.ServiceProviderId.Value,
                        AssignTo = o.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = o.RequestConcern.ServiceProvider.ServiceProviderName,
                        CreatedTime = o.RequestConcern.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        Severity = o.RequestConcern.Severity,
                        DateStarted = o.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        RequestConcernId = o.RequestConcernId



                    }).ToListAsync();

                var transferTicketQuery = await _context.TransferTicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsTransfer == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketExportResult
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
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy HH:mm"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.TransferAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        Target_Date = ct.Current_Target_Date.Value.Date.ToString("MM/dd/yyyy"),
                        Ticket_Status = "Transfer",
                        Remarks = ct.TransferRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TransferToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        CreatedTime = ct.TicketConcern.RequestConcern.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        Severity = ct.TicketConcern.RequestConcern.Severity,
                        DateStarted = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        RequestConcernId = ct.TicketConcern.RequestConcernId,
                    }).ToListAsync();

                var onHoldTicketQuery = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsHold == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketExportResult
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
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy HH:mm"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.TicketConcern.RequestConcern.Notes,
                        Transaction_Date = ct.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        Target_Date = ct.TicketConcern.TargetDate.Value.Date.ToString("MM/dd/yyyy"),
                        Ticket_Status = "On-Hold",
                        Remarks = ct.OnHoldRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, DateTime.Now.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        CreatedTime = ct.TicketConcern.RequestConcern.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        Severity = ct.TicketConcern.RequestConcern.Severity,
                        DateStarted = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        RequestConcernId = ct.TicketConcern.RequestConcernId,

                    }).ToListAsync();

                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsClosing == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= request.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= request.Date_To.Value.Date)
                    .Select(ct => new AllTicketExportResult
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
                        Date_Needed = ct.TicketConcern.RequestConcern.DateNeeded.Value.ToString("MM/dd/yyyy HH:mm"),
                        Contact_Number = ct.TicketConcern.RequestConcern.ContactNumber,
                        Notes = ct.Notes,
                        Transaction_Date = ct.ClosingAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        Target_Date = ct.TicketConcern.TargetDate.Value.Date.ToString("MM/dd/yyyy"),
                        Ticket_Status = "Closed",
                        Remarks = ct.ClosingRemarks,
                        Aging_Days = EF.Functions.DateDiffDay(ct.TicketConcern.DateApprovedAt.Value.Date, ct.ClosingAt.Value.Date),
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId.Value,
                        StartDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ForClosingDate = ct.ForClosingAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ClosedDate = ct.ClosingAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        ServiceProvider = ct.TicketConcern.RequestConcern.ServiceProviderId.Value,
                        AssignTo = ct.TicketConcern.RequestConcern.AssignToUser.Fullname,
                        ServiceProviderName = ct.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ClosingStatus = ct.TicketConcern.TargetDate.Value.Date < ct.TicketConcern.Closed_At.Value.Date ? "Delayed" : "On-Time",
                        Technician1 = ct.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(0).Take(1).FirstOrDefault(),
                        Technician2 = ct.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(1).Take(1).FirstOrDefault(),
                        Technician3 = ct.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(2).Take(1).FirstOrDefault(),
                        Resolution = ct.TicketConcern.RequestConcern.Resolution,
                        CreatedTime = ct.TicketConcern.RequestConcern.CreatedAt.ToString("MM/dd/yyyy HH:mm"),
                        CompletedTime = ct.TicketConcern.Closed_At.Value.ToString("MM/dd/yyyy HH:mm"),
                        Severity = ct.TicketConcern.RequestConcern.Severity,
                        DateStarted = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy HH:mm"),
                        RequestConcernId = ct.TicketConcern.RequestConcernId,

                    }).ToListAsync();




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



                var results = combineTicketReports
                    .OrderBy(x => x.Transaction_Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(r => new AllTicketExportResult
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
                        ForClosingDate = r.ForClosingDate,
                      
                        ClosedDate = r.ClosedDate,
                        AssignTo = r.AssignTo,
                        ServiceProvider = r.ServiceProvider,
                        ServiceProviderName = r.ServiceProviderName,
                        ClosingStatus = r.ClosingStatus,
                        Personnel_Id = r.Personnel_Id,
                        Technician1 = r.Technician1,
                        Technician2 = r.Technician2,
                        Technician3 = r.Technician3,
                        Resolution = r.Resolution,
                        CreatedTime = r.CreatedTime,
                        CompletedTime = r.CompletedTime,
                        DateStarted = r.DateStarted,
                        Severity = r.Severity,
                        RequestConcernId = r.RequestConcernId,

                    }).ToList();


                if (request.ServiceProvider is not null)
                {
                    results = results
                           .Where(x => x.ServiceProvider == request.ServiceProvider)
                           .ToList();

                    if (request.Channel is not null)
                    {
                        results = results
                           .Where(x => x.ChannelId == request.Channel)
                           .ToList();

                        if (request.UserId is not null)
                        {
                            results = results
                                .Where(x => x.Personnel_Id == request.UserId)
                                .ToList();
                        }
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketConcernId.Contains(request.Search)
                        || x.Personnel.ToLower().Contains(request.Search)
                        || x.Request_Type.ToLower().Contains(request.Search)
                        || x.BackJobId.Contains(request.Search)
                        || x.Requestor_Name.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Concerns.ToLower(), @"\s+", " ").Contains(normalizedSearch)
                        || x.AssignTo.ToLower().Contains(request.Search)).ToList();
                }

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"All Ticket Report");
                    var headers = new List<string>
                    {
                        "Technician",
                        "Technician 2",
                        "Technician 3",
                        "Category",
                        "Subcategory",
                        "Item",
                        "Subject",
                        "Description",
                        "Asset Tag #",
                        "Service Provider",
                        "MIR #",
                        "Material Cost",
                        "Resolution",
                        "Request Mode",
                        "Request Type",
                        "Request Status",
                        "First Response OverDue Status",
                        "Overdue Status",
                        "Work Site",
                        "Requester",
                        "Company",
                        "Department",
                        "Location",
                        "Created Time",
                        "Completed Time",
                        "Time Elapsed",
                        "Priority",
                        "Date Created / Received",
                        "Date Needed",
                        "Date Started",
                        "Date For Closed",
                        "Date Finished",
                        "Request ID",
                        "Customer Satisfactionn",
                        "Comment / Suggestions"





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

                        row.Cell(1).Value = results[index - 1].Technician1;
                        row.Cell(2).Value = results[index - 1].Technician2;
                        row.Cell(3).Value = results[index - 1].Technician3;
                        row.Cell(4).Value = results[index - 1].TicketCategoryDescriptions;
                        row.Cell(5).Value = results[index - 1].TicketSubCategoryDescriptions;
                        //row.Cell(6).Value = results[index - 1].Item;
                        //row.Cell(7).Value = results[index - 1].Subject;
                        row.Cell(8).Value = results[index - 1].Concerns;
                        //row.Cell(9).Value = results[index - 1].AssetTag;
                        row.Cell(10).Value = results[index - 1].ServiceProviderName;
                        //row.Cell(11).Value = results[index - 1]."MIR";
                        //row.Cell(12).Value = results[index - 1].materialCost;
                        row.Cell(13).Value = results[index - 1].Resolution;
                        //row.Cell(14).Value = results[index - 1].RequestMode;
                        row.Cell(15).Value = results[index - 1].Request_Type;
                        row.Cell(16).Value = results[index - 1].Ticket_Status;
                        //row.Cell(17).Value = results[index - 1].FirstResponse;
                        //row.Cell(18).Value = results[index - 1].OverdueStatus;
                        //row.Cell(19).Value = results[index - 1].WorkSite;
                        row.Cell(20).Value = results[index - 1].Requestor_Name;
                        row.Cell(21).Value = $"{results[index - 1].Company_Code} - {results[index - 1].Company_Name}";
                        row.Cell(22).Value = $"{results[index - 1].Department_Code} - {results[index - 1].Department_Name}";
                        row.Cell(23).Value = $"{results[index - 1].Location_Code} - {results[index - 1].Location_Name}";
                        row.Cell(24).Value = results[index - 1].CreatedTime;
                        row.Cell(25).Value = results[index - 1].CompletedTime;
                        //row.Cell(26).Value = results[index - 1].TimeElapsed;
                        row.Cell(27).Value = results[index - 1].Severity;
                        row.Cell(28).Value = results[index - 1].CreatedTime;
                        row.Cell(29).Value = results[index - 1].Date_Needed;

                        row.Cell(30).Value = results[index - 1].DateStarted;
                        row.Cell(31).Value = results[index - 1].ForClosingDate;
                        row.Cell(32).Value = results[index - 1].CompletedTime;
                        row.Cell(33).Value = results[index - 1].TicketConcernId;
                        //row.Cell(34).Value = results[index - 1].Severity;
                        //row.Cell(35).Value = results[index - 1].CreatedTime;



                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"AllTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }

    }
}
