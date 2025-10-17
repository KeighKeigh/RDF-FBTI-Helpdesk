using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket
{
    
    public partial class GetOpenTicket
    {
        public class Handler : IRequestHandler<GetOpenTicketQuery, PagedList<GetOpenTicketResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOpenTicketResult>> Handle(GetOpenTicketQuery request, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                IQueryable<TicketConcern> ticketConcernQuery = _context.TicketConcerns
                    .AsNoTracking()
                    .Include(x => x.ClosingTickets)
                    .Include(x => x.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.AssignTo == request.UserId);




                if (!string.IsNullOrEmpty(request.Concern_Status))
                {
                    switch (request.Concern_Status)
                    {
                        case TicketingConString.PendingRequest:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == false && x.OnHold == null);
                            break;

                        case TicketingConString.ForApprovalTicket:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == false && x.ConcernStatus == TicketingConString.ForApprovalTicket && x.IsAssigned == true);
                            break;

                        case TicketingConString.Open:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == true && x.ConcernStatus == TicketingConString.OnGoing && x.IsClosedApprove == null && x.IsTransfer == null && x.OnHold == null || x.IsTransfer != false
                                && x.IsClosedApprove == null && x.OnHold == null && x.IsApprove == true);
                            break;

                        case TicketingConString.ForTransfer:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == null)
                                .Where(x => x.TransferTicketConcerns
                                .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
                                .TransferBy == request.UserId);
                            break;

                        case TicketingConString.ForOnHold:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == false);
                            break;

                        case TicketingConString.OnHold:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == true);
                            break;

                        case TicketingConString.ForClosing:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsClosedApprove == false && x.OnHold == null);
                            break;

                        case TicketingConString.NotConfirm:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
                            break;

                        case TicketingConString.Closed:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.RequestConcern.Is_Confirm == true);
                            break;

                        case TicketingConString.DateRejected:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.ConcernStatus == TicketingConString.DateRejected);
                            break;

                        default:
                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                    }
                }

                if (!string.IsNullOrEmpty(request.History_Status))
                {
                    switch (request.History_Status)
                    {
                        case TicketingConString.PendingRequest:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == false && x.OnHold == null);
                            break;

                        case TicketingConString.Open:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsApprove == true || x.IsTransfer != false
                                && x.IsClosedApprove == null && x.OnHold == null);
                            break;

                        case TicketingConString.ForTransfer:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == null)
                                .Where(x => x.TransferTicketConcerns
                                .FirstOrDefault(x => x.IsActive == true && x.IsTransfer == false)
                                .TransferBy == request.UserId);
                            break;

                        case TicketingConString.ForOnHold:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == false);
                            break;

                        case TicketingConString.OnHold:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.OnHold == true);
                            break;

                        case TicketingConString.ForClosing:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsClosedApprove == false && x.OnHold == null);
                            break;

                        case TicketingConString.NotConfirm:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null);
                            break;

                        case TicketingConString.Closed:
                            ticketConcernQuery = ticketConcernQuery
                                .Where(x => x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null);
                            break;

                        default:
                            return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                    }
                }

                if (request.Date_From is not null && request.Date_To is not null)
                {
                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.TargetDate >= request.Date_From.Value && x.TargetDate <= request.Date_To.Value);
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    ticketConcernQuery = ticketConcernQuery
                        .Where(x => x.User.Fullname.ToLower().Contains(request.Search.ToLower())
                            || x.Id.ToString().Contains(request.Search)
                            || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
                }

                if (request.Ascending == false)
                {
                    ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.Id);
                }


                if (request.AscendingDate == false)
                {
                    ticketConcernQuery = ticketConcernQuery.OrderByDescending(x => x.TargetDate);
                }





                var totalCount = await ticketConcernQuery.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    return new PagedList<GetOpenTicketResult>(new List<GetOpenTicketResult>(), 0, request.PageNumber, request.PageSize);
                }

                //var totalCount = await ticketConcernQuery.CountAsync(cancellationToken);

                var results = await ticketConcernQuery
                    .AsNoTracking()
                    .Select(x => new GetOpenTicketResult
                    {
                       
                        TicketConcernId = x.Id,
                        RequestConcernId = x.RequestConcernId,
                        Severity = x.RequestConcern.Severity,
                        CompanyId = x.RequestConcern.CompanyId,
                        DepartmentId = x.RequestConcern.DepartmentId,
                        LocationId = x.RequestConcern.LocationId,
                        Concern_Description = x.RequestConcern.Concern,
                        Department_Code = x.RequestorByUser.OneChargingMIS.department_code,
                        Department_Name = x.RequestorByUser.OneChargingMIS.department_name,
                        Location_Code = x.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = x.RequestConcern.OneChargingMIS.location_name,
                        Requestor_By = x.RequestorBy,
                        Requestor_Name = x.RequestorByUser.Fullname,
                        IsStore = x.RequestConcern.User.IsStore,
                        DateStarted = x.DateApprovedAt,
                        GetOpenTicketCategories = x.RequestConcern.TicketCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketCategory
                                                {
                                                    TicketCategoryId = t.Id,
                                                    CategoryId = t.CategoryId,
                                                    Category_Description = t.Category.CategoryDescription,
                                                }),

                        GetOpenTicketSubCategories = x.RequestConcern.TicketSubCategories
                                                .Select(t => new GetOpenTicketResult.GetOpenTicketSubCategory
                                                {
                                                    TicketSubCategoryId = t.Id,
                                                    SubCategoryId = t.SubCategoryId,
                                                    SubCategory_Description = t.SubCategory.SubCategoryDescription,
                                                }),

                        Date_Needed = x.RequestConcern.DateNeeded,
                        Notes = x.RequestConcern.Notes,
                        Contact_Number = x.RequestConcern.ContactNumber,
                        Request_Type = x.RequestConcern.RequestType,
                        BackJobId = x.RequestConcern.BackJobId,
                        Back_Job_Concern = x.RequestConcern.BackJob.Concern,
                        ChannelId = x.RequestConcern.ChannelId,
                        Channel_Name = x.RequestConcern.Channel.ChannelName,
                        ServiceProviderId = x.RequestConcern.ServiceProviderId,
                        ServiceProviderName = x.RequestConcern.ServiceProvider.ServiceProviderName,
                        TransferChannelId = x.RequestConcern.TransferChannelId,
                        Transfer_Channel_Name = x.RequestConcern.TransferChannel.ChannelName,
                        UserId = x.UserId,
                        Issue_Handler = x.User.Fullname,
                        Target_Date = x.TargetDate,
                        Ticket_Status = x.IsApprove == false && x.OnHold == null ? TicketingConString.PendingRequest
                                        : x.IsApprove == true != false && x.IsTransfer != false && x.IsClosedApprove == null && x.OnHold == null ? TicketingConString.OpenTicket
                                        : x.TransferTicketConcerns.FirstOrDefault(x => x.IsTransfer == false && x.IsActive == true)
                                        .TransferBy == request.UserId ? TicketingConString.ForTransfer
                                        : x.OnHold == false ? TicketingConString.ForOnHold
                                        : x.OnHold == true ? TicketingConString.OnHold
                                        : x.IsClosedApprove == false && x.OnHold == null ? TicketingConString.ForClosing
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == null && x.OnHold == null ? TicketingConString.NotConfirm
                                        : x.IsClosedApprove == true && x.RequestConcern.Is_Confirm == true && x.OnHold == null ? TicketingConString.Closed
                                        : "Unknown",

                        Added_By = x.AddedByUser.Fullname,
                        Created_At = x.CreatedAt,
                        Remarks = x.Remarks,
                        Modified_By = x.ModifiedByUser.Fullname,
                        Updated_At = x.UpdatedAt,
                        IsActive = x.IsActive,
                        Is_Closed = x.IsClosedApprove,
                        Closed_At = x.RequestConcern.Is_Confirm == null ? null : x.Closed_At,
                        Is_Transfer = x.IsTransfer,
                        Transfer_At = x.TransferAt,
                        GetForClosingTickets =  x.ClosingTickets
                       .Where(x => x.IsActive == true && x.IsRejectClosed == false)
                       .Where(x => x.IsClosing == true ? x.IsClosing == true : x.IsClosing == false)
                      .Select(x => new GetOpenTicketResult.GetForClosingTicket
                      {
                          ClosingTicketId = x.Id,
                          Remarks = x.RejectRemarks,
                          Resolution = x.Resolution,
                          CategoryConcernId = x.CategoryConcernId,
                          CategoryConcernName = x.CategoryConcernName,

                          Closed_Status = x.ForClosingAt != null ? x.TicketConcern.TargetDate.Value.Date >= x.ForClosingAt.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay 
                          : x.TicketConcern.TargetDate.Value.Date >= x.ClosingAt.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,

                          ForClosingTicketTechnicians = x.ticketTechnicians.
                          Select(t => new GetOpenTicketResult.GetForClosingTicket.ForClosingTicketTechnician
                          {
                              TicketTechnicianId = t.Id,
                              Technician_By = t.TechnicianBy,
                              Fullname = t.TechnicianByUser.Fullname,
                          }),

                          Notes = x.Notes,
                          IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
                          ApproverLists = x.ApproverTickets
                          .Where(x => x.IsApprove != null)
                          .Select(x => new ApproverList
                          {
                              ApproverName = x.User.Fullname,
                              Approver_Level = x.ApproverLevel.Value
                          }),

                          GetAttachmentForClosingTickets = x.TicketAttachments.Select(x => new GetAttachmentForClosingTicket
                          {
                              TicketAttachmentId = x.Id,
                              Attachment = x.Attachment,
                              FileName = x.FileName,
                              FileSize = x.FileSize,
                          }),
                      }),

                        GetForOnHolds =  x.TicketOnHolds
                        .Where(x => x.IsHold == false && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetForOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentForOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetForOnHold.GetAttachmentForOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,
                            }),
                        }),

                        GetOnHolds =  x.TicketOnHolds
                        .Where(x => x.IsHold == true && x.IsActive)
                        .Select(h => new GetOpenTicketResult.GetOnHold
                        {
                            Id = h.Id,
                            Reason = h.Reason,
                            AddedBy = h.AddedByUser.Fullname,
                            CreatedAt = h.CreatedAt,
                            IsHold = h.IsHold,
                            ResumeAt = h.ResumeAt,
                            IsApprove = h.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentOnHoldOpenTickets = h.TicketAttachments
                            .Select(t => new GetOpenTicketResult.GetOnHold.GetAttachmentOnHoldOpenTicket
                            {
                                TicketAttachmentId = t.Id,
                                Attachment = t.Attachment,
                                FileName = t.FileName,
                                FileSize = t.FileSize,
                            }),
                        }),

                        GetForTransferTickets =  x.TransferTicketConcerns
                        .Where(x => x.IsActive == true && x.IsTransfer == false && x.TransferBy == request.UserId)
                        .Select(x => new GetOpenTicketResult.GetForTransferTicket
                        {
                            TransferTicketConcernId = x.Id,
                            Transfer_Remarks = x.TransferRemarks,
                            Transfer_To = x.TransferTo,
                            Transfer_To_Name = x.TransferToUser.Fullname,
                            Current_Target_Date = x.Current_Target_Date,
                            IsApprove = x.ApproverTickets.Any(x => x.IsApprove == true),
                            GetAttachmentForTransferTickets = x.TicketAttachments.Select(x => new GetOpenTicketResult.GetForTransferTicket.GetAttachmentForTransferTicket
                            {
                                TicketAttachmentId = x.Id,
                                Attachment = x.Attachment,
                                FileName = x.FileName,
                                FileSize = x.FileSize,
                            }),
                        }),

                        Transaction_Date = x.ticketHistories.Max(x => x.TransactionDate).Value,

                        Aging_Days = x.RequestConcern.Is_Confirm == null ? null : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date) <= 0 ? 0 : EF.Functions.DateDiffDay(x.DateApprovedAt.Value.Date, x.Closed_At.Value.Date),


                    }).ToListAsync(cancellationToken);

                return new PagedList<GetOpenTicketResult>(results, totalCount, request.PageNumber, request.PageSize);
            }
        }
    }
}

