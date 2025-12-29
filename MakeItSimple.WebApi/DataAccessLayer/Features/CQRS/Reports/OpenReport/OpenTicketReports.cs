using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport
{
    public partial class OpenTicketReports
    {

        public class Handler : IRequestHandler<OpenTicketReportsQuery, PagedList<OpenTicketReportsResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<OpenTicketReportsResult>> Handle(OpenTicketReportsQuery request, CancellationToken cancellationToken)
            {
                var requestConcernList = await _context.RequestConcerns
                    .AsNoTracking()
                    .Where(x => x.IsActive && x.BackJobId != null).ToListAsync();

                var backJobIds = requestConcernList.Select(x => x.BackJobId.Value).Distinct().ToList();

                var requestConcernWithBackjob = await _context.TicketConcerns
                    .AsNoTracking()
                    .Where(x => backJobIds.Contains(x.Id)).ToListAsync();


                var results = _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.RequestorByUser)
                    .Include(x => x.User)
                    .Include(x => x.ClosingTickets)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.TransferTicketConcerns)
                    .ThenInclude(x => x.TicketAttachments)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketCategories)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.RequestConcern)
                    .ThenInclude(x => x.TicketSubCategories)
                    .ThenInclude(x => x.SubCategory)

                    .AsSplitQuery()
                    .Where(x => x.IsApprove == true && x.IsClosedApprove != true && x.OnHold != true && x.IsTransfer != true && x.IsDone != true)
                    //.Where(x => x.DateApprovedAt.Value.Date >= request.Date_From.Value.Date && x.DateApprovedAt.Value.Date <= request.Date_To.Value.Date)

                    .Select(t => new OpenTicketReportsResult
                    {
                        TicketConcernId = t.Id,
                        Concern_Description = t.RequestConcern.Concern,
                        Requestor_Name = t.RequestorByUser.Fullname,
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
                        //Category_Description = string.Join(", ", t.RequestConcern.Category.CategoryDescription),
                        Category_Description = string.Join(", ", t.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory_Description = string.Join(", ", t.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Issue_Handler = t.User.Fullname,
                        Channel_Name = t.RequestConcern.Channel.ChannelName,
                        Target_Date = t.TargetDate,
                        Created_At = t.CreatedAt,
                        Modified_By = t.ModifiedByUser.Fullname,
                        Updated_At = t.UpdatedAt,
                        Remarks = t.Remarks,
                        Aging_Days = EF.Functions.DateDiffDay(t.TargetDate.Value.Date, DateTime.Now.Date),
                        Personnel_Unit = t.User.UnitId,
                        Personnel_Id = t.User.Id,
                        Personnel = t.User.Fullname,
                        ChannelId = t.RequestConcern.ChannelId,
                        DatePicked = t.RequestConcern.DatePicked,
                        StartDate = t.DateApprovedAt,
                        ServiceProvider = t.RequestConcern.ServiceProviderId,
                        AssigTo = t.RequestConcern.AssignToUser.Fullname,
                        AssignTo = t.AssignTo,
                        ConcernCategory = t.RequestConcern.CategoryConcernName,
                        RequestType = t.RequestConcern.RequestType,
                        AgingDays = EF.Functions.DateDiffDay(t.TargetDate.Value.Date, DateTime.Now.Date) <= 0 ? 0 : EF.Functions.DateDiffDay(t.TargetDate.Value.Date, t.Closed_At.Value.Date),
                        Rating = EF.Functions.DateDiffDay(t.DateApprovedAt.Value.Date, DateTime.Now.Date) >= 31 ? 1
                        : EF.Functions.DateDiffDay(t.DateApprovedAt.Value.Date, DateTime.Now.Date) >= 15 ? 2
                        : 3,
                        Year = t.TargetDate.Value.Year,
                        Month = t.TargetDate.Value.Month,
                        Backjobs = 3
                        





                    });

                if (request.ServiceProvider is not null)
                {
                    results = results.Where(x => x.ServiceProvider == request.ServiceProvider);

                    if (request.Channel is not null)
                    {
                        results = results.Where(x => x.ChannelId == request.Channel);

                        if (request.UserId is not null)
                        {
                            results = results.Where(x => x.AssignTo == request.UserId);
                        }
                    }
                }

                foreach (var ticket in results)
                {
                    ticket.Backjobs = requestConcernWithBackjob.Count(x => x.Id == ticket.TicketConcernId) >= 3 ? 1
                        : requestConcernWithBackjob.Count(x => x.Id == ticket.TicketConcernId) >= 1 ? 2
                        : 3;
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    results = results
                        .Where(x => x.TicketConcernId.ToString().Contains(request.Search)
                        || x.Concern_Description.Contains(request.Search)
                        || x.Requestor_Name.Contains(request.Search)
                        || x.CompanyName.Contains(request.Search)
                        || x.Business_Unit_Name.Contains(request.Search)
                        || x.Department_Name.Contains(request.Search)
                        || x.Unit_Name.Contains(request.Search)
                        || x.SubUnit_Name.Contains(request.Search)
                        || x.Location_Name.Contains(request.Search)
                        || x.Category_Description.Contains(request.Search)
                        || x.SubCategory_Description.Contains(request.Search)
                        || x.Issue_Handler.Contains(request.Search)
                        || x.Channel_Name.Contains(request.Search)
                        || x.Modified_By.Contains(request.Search));

                }


                var final = results
                    .OrderBy(x => x.Target_Date.Value.Date)
                    .ThenBy(x => x.TicketConcernId)
                    .Select(f => new OpenTicketReportsResult
                    {
                        TicketConcernId = f.TicketConcernId,
                        Concern_Description = f.Concern_Description,
                        Requestor_Name = f.Requestor_Name,
                        CompanyCode = f.CompanyCode,
                        CompanyName = f.CompanyName,
                        Business_Unit_Code = f.Business_Unit_Code,
                        Business_Unit_Name = f.Business_Unit_Name,
                        Department_Code = f.Department_Code,
                        Department_Name = f.Department_Name,
                        Unit_Code = f.Unit_Code,
                        Unit_Name = f.Unit_Name,
                        SubUnit_Code = f.SubUnit_Code,
                        SubUnit_Name = f.SubUnit_Name,
                        Location_Code = f.Location_Code,
                        Location_Name = f.Location_Name,
                        Category_Description = f.Category_Description,
                        SubCategory_Description = f.SubCategory_Description,
                        Issue_Handler = f.Issue_Handler,
                        Channel_Name = f.Channel_Name,
                        Target_Date = f.Target_Date,
                        Created_At = f.Created_At,
                        Modified_By = f.Modified_By,
                        Updated_At = f.Updated_At,
                        Remarks = f.Remarks,
                        DatePicked = f.DatePicked,
                        Aging_Days = f.Aging_Days,
                        Personnel_Unit = f.Personnel_Unit,
                        Personnel_Id = f.Personnel_Id,
                        Personnel = f.Personnel,
                        ChannelId = f.ChannelId,
                        StartDate = f.StartDate,
                        AssigTo = f.AssigTo,
                        Backjobs = f.Backjobs,
                        Year = f.Year,
                        Month = f.Month,
                        RequestType = f.RequestType,
                        AgingDays = f.AgingDays,
                        Rating = f.Rating,
                        
                        

                    }).AsQueryable();

                return await PagedList<OpenTicketReportsResult>.CreateAsync(final, request.PageNumber, request.PageSize);
            }

        }
    }
}
