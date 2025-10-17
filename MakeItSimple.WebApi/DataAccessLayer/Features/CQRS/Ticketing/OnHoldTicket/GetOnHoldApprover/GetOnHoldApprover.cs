using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OpenTicketConcern.GetOpenTicketSubUnit.GetOpenTicketSubUnit;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.OnHoldTicket.GetOnHoldApprover
{
    public class GetOnHoldApprover
    {

        public class GetOnHoldApproverQuery : UserParams, IRequest<PagedList<GetOnHoldApproverResult>>
        {
            public Guid? UserId { get; set; }
            public string UserType { get; set; }
            public string TicketStatus { get; set; }
            public string Role { get; set; }
            public string Search { get; set; }
        }

        public class GetOnHoldApproverResult
        {
            public int TicketConcernId { get; set; }
            public string Concern_Details { get; set; }
            public string Resolution { get; set; }
            public string Notes { get; set; }
            public int? DepartmentId { get; set; }
            public string Department_Name { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string TicketStatus { get; set; }

            public string Reason { get; set; }
            public List<GetOnHoldApproverCategory> GetOnHoldApproverCategories { get; set; }

            public class GetOnHoldApproverCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetOnHoldApproverSubCategory> GetOnHoldApproverSubCategories { get; set; }

            public class GetOnHoldApproverSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }

            public string SubCategoryDescription { get; set; }
            public int? Delay_Days { get; set; }
            public string Added_By { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? Start_Date { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime? Hold_Date { get; set; }
            public DateTime? Approved_Date { get; set; }

            public List<OpenTicketSubCategoryAttachment> OpenTicketSubCategoryAttachments { get; set; }

            public class OpenTicketSubCategoryAttachment
            {
                public int? TicketAttachmentId { get; set; }
                public string Attachment { get; set; }
                public string FileName { get; set; }
                public decimal? FileSize { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }
            }
        }

        public class Handler : IRequestHandler<GetOnHoldApproverQuery, PagedList<GetOnHoldApproverResult>>
        {
            private readonly MisDbContext _context;

            public Handler (MisDbContext context)
            {
                _context = context;
            }


            public async Task<PagedList<GetOnHoldApproverResult>> Handle(GetOnHoldApproverQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                IQueryable<TicketOnHold> onHoldTickets = _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution();


                if (onHoldTickets.Any())
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
                        onHoldTickets = onHoldTickets
                            .Where(x => x.AddedByUser.Fullname.Contains(request.Search)
                                        || x.ToString().Contains(request.Search));
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        var filterApproval = onHoldTickets.Select(x => x.Id);

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
                                        x.User,
                                        x.TicketConcernId
                                    }).ToListAsync();

                                var userRequestIdApprovalList = approverTransactList.Select(x => x.TicketConcernId);
                                var userIdsInApprovalList = approverTransactList.Select(approval => approval.User.Fullname);

                                onHoldTickets = onHoldTickets
                                    .Where(x => userIdsInApprovalList.Contains(x.ApprovedBy)
                                    && userRequestIdApprovalList.Contains(x.TicketConcernId));
                            }
                        }
                        else if (request.UserType == TicketingConString.Users)
                        {
                            onHoldTickets = onHoldTickets.Where(x => x.AddedByUser.Id == request.UserId);
                        }
                        else
                        {
                            return new PagedList<GetOnHoldApproverResult>(new List<GetOnHoldApproverResult>(), 0, request.PageNumber, request.PageSize);
                        }
                    }
                }


                var results = onHoldTickets
                    .OrderByDescending(x => x.TicketConcern.TargetDate)
                    .Select(x => new GetOnHoldApproverResult
                    {
                        TicketConcernId = x.Id,
                        Concern_Details = x.TicketConcern.RequestConcern.Concern,
                        Notes = x.TicketConcern.RequestConcern.Notes,
                        DepartmentId = x.AddedByUser.DepartmentId,
                        Department_Name = x.AddedByUser.OneChargingMIS.department_name,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        Channel_Name = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        UserId = x.AddedBy,
                        Fullname = x.AddedByUser.Fullname,
                        Reason = x.Reason,
                        TicketStatus = TicketingConString.OnHold,
                        GetOnHoldApproverCategories = x.TicketConcern.RequestConcern.TicketCategories
                        .Select(t => new GetOnHoldApproverResult.GetOnHoldApproverCategory
                        {
                            TicketCategoryId = t.Id,
                            CategoryId = t.CategoryId,
                            Category_Description = t.Category.CategoryDescription,
                        }).ToList(),

                        GetOnHoldApproverSubCategories = x.TicketConcern.RequestConcern.TicketSubCategories
                        .Select(t => new GetOnHoldApproverResult.GetOnHoldApproverSubCategory
                        {
                            TicketSubCategoryId = t.Id,
                            SubCategoryId = t.SubCategoryId,
                            SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        }).ToList(),

                        SubCategoryDescription = x.TicketConcern.RequestConcern.SubCategory.SubCategoryDescription,
                        Target_Date = x.TicketConcern.TargetDate,
                        Start_Date = x.TicketConcern.DateApprovedAt,
                        Added_By = x.AddedByUser.Fullname,
                        Updated_At = x.TicketConcern.UpdatedAt,
                        Hold_Date = x.CreatedAt,
                        Approved_Date = x.ApprovedAt,
                        Modified_By = x.TicketConcern.ModifiedByUser.Fullname,
                        Delay_Days = x.TicketConcern.TargetDate < dateToday ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate, dateToday) : 0,

                        OpenTicketSubCategoryAttachments = x.TicketAttachments.Select(x => new GetOnHoldApproverResult.OpenTicketSubCategoryAttachment
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

                return await PagedList<GetOnHoldApproverResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
