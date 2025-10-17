using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.CompanySetup.GetCompany.GetCompanyResult;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.GetClosing.GetClosingTicket;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit
{
    public partial class GetOpenTicketSubUnit
    {
        public class Handler : IRequestHandler<GetOpenTicketSubUnitQuery, PagedList<GetOpenTicketSubUnitResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOpenTicketSubUnitResult>> Handle(GetOpenTicketSubUnitQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<TicketConcern> openTicketsQuery = _context.TicketConcerns
                    .Where(x => x.ConcernStatus == TicketingConString.OnGoing && x.RequestConcern.IsDone != true && x.OnHold != true)
                    .AsNoTrackingWithIdentityResolution()
                    .AsSplitQuery();

                

                if (!string.IsNullOrEmpty(request.TicketStatus))
                {
                    if (request.TicketStatus.Equals("Delayed", StringComparison.OrdinalIgnoreCase))
                    {

                        openTicketsQuery = openTicketsQuery
                            .Where(x => x.TargetDate.Value.Date < dateToday);
                    }

                }

                IQueryable<ClosingTicket> isClosingTIcket = _context.ClosingTickets.Where(x => x.IsClosing == false && x.IsRejectClosed == false).AsNoTrackingWithIdentityResolution().AsSplitQuery();

                IQueryable<TicketOnHold> onHoldTicket = _context.TicketOnHolds.Where(x => x.IsHold == false && x.IsRejectOnHold == false).AsNoTrackingWithIdentityResolution().AsSplitQuery();

                IQueryable<TransferTicketConcern> transferTIcket = _context.TransferTicketConcerns.Where(x => x.IsTransfer == false && x.IsRejectTransfer == false).AsNoTrackingWithIdentityResolution().AsSplitQuery();


                if (openTicketsQuery.Any())
                {
                    var allUserList = await _context.UserRoles
                        .AsNoTracking()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions
                        }).ToListAsync();


                    var approverPermissionList = allUserList
                        .Where(x => x.Permissions
                        .Contains(TicketingConString.Approver))
                        .Select(x => x.UserRoleName)
                        .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        openTicketsQuery = openTicketsQuery
                            .Where(x => x.User.Fullname.Contains(request.Search)
                                        || x.ToString().Contains(request.Search));
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = openTicketsQuery.Select(x => x.Id);

                        if (request.UserType == TicketingConString.Approver)
                        {
                            if (approverPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                var userApprover = await _context.Users
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                                var approverTransactList = await _context.ApproverTicketings
                                    .AsNoTracking()
                                    .Where(x => x.UserId == userApprover.Id)
                                    .Where(x => x.IsApprove == true)
                                    .Select(x => new
                                    {
                                        //x.ApproverLevel,
                                        x.IsApprove,
                                        x.ApproverDateId,
                                        x.UserId,
                                        x.TicketConcernId
                                    }).ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.TicketConcernId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.UserId);

                                openTicketsQuery = openTicketsQuery
                                    .Where(x => userIdsInApprovalList.Contains(x.ApprovedDateBy)
                                    && userRequestIdApprovalList.Contains(x.Id));
                            }
                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            openTicketsQuery = openTicketsQuery.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<GetOpenTicketSubUnitResult>(new List<GetOpenTicketSubUnitResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                }


                if(request.AscendingDate == true)
                {
                    openTicketsQuery = openTicketsQuery.OrderBy(x => x.TargetDate);
                }
                else
                {
                    openTicketsQuery = openTicketsQuery.OrderByDescending(x => x.TargetDate);
                }
                var results = openTicketsQuery
                    .Select(x => new GetOpenTicketSubUnitResult
                    {
                        TicketConcernId = x.Id,
                        Concern_Details = x.RequestConcern.Concern,
                        Notes = x.RequestConcern.Notes,
                        DepartmentId = x.User.DepartmentId,
                        Department_Name = x.User.OneChargingMIS.department_name,
                        ChannelId = x.RequestConcern.ChannelId,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        UserId = x.UserId,
                        Fullname = x.User.Fullname,
                        TicketStatus = isClosingTIcket.Any(closing => closing.TicketConcernId == x.Id ) ? TicketingConString.ForClosing
                        : onHoldTicket.Any(hold => hold.TicketConcernId == x.Id) ? TicketingConString.ForOnHold
                        : transferTIcket.Any(trans => trans.TicketConcernId == x.Id) ? TicketingConString .ForTransfer
                        : x.ConcernStatus,

                        GetOpenTicketSubUnitCategories = x.RequestConcern.TicketCategories
                        .Select(t => new GetOpenTicketSubUnitResult.GetOpenTicketSubUnitCategory
                        {
                            TicketCategoryId = t.Id,
                            CategoryId = t.CategoryId,
                            Category_Description = t.Category.CategoryDescription,
                        }).ToList(),

                        GetOpenTicketSubUnitSubCategories = x.RequestConcern.TicketSubCategories
                        .Select(t => new GetOpenTicketSubUnitResult.GetOpenTicketSubUnitSubCategory
                        {
                            TicketSubCategoryId = t.Id,
                            SubCategoryId = t.SubCategoryId,
                            SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        }).ToList(),

                        SubCategoryDescription = x.RequestConcern.SubCategory.SubCategoryDescription,
                        Target_Date = x.TargetDate,
                        Start_Date = x.DateApprovedAt,
                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Updated_At = x.UpdatedAt,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Delay_Days = x.TargetDate < dateToday  ? EF.Functions.DateDiffDay(x.TargetDate, dateToday) : 0,

                        OpenTicketSubCategoryAttachments = x.TicketAttachments.Select(x => new GetOpenTicketSubUnitResult.OpenTicketSubCategoryAttachment
                        {
                            TicketAttachmentId = x.Id,
                            Attachment = x.Attachment,
                            FileName = x.FileName,
                            FileSize = x.FileSize,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt,
                        }).ToList()
                    });

                return await PagedList<GetOpenTicketSubUnitResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
