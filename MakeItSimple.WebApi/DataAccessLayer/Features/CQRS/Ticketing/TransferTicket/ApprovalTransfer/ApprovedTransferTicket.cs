using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TransferTicket.ApprovalTransfer
{
    public partial class ApprovedTransferTicket
    {
        public class Handler : IRequestHandler<ApprovedTransferTicketCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await _context.Users
                    .FirstOrDefaultAsync(x => x.Id == command.Transacted_By);

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

                var transferTicketExist = await _context.TransferTicketConcerns
                    .Include(x => x.TicketConcern)
                    .ThenInclude(x => x.RequestConcern)
                    .FirstOrDefaultAsync(x => x.Id == command.TransferTicketId, cancellationToken);

                if (transferTicketExist is null)
                    return Result.Failure(TransferTicketError.TransferTicketConcernIdNotExist());

                if(transferTicketExist.IsActive is false )
                    return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                var transferApprover = await _context.ApproverTicketings
                    .Where(x => x.TransferTicketConcernId == transferTicketExist.Id && x.IsApprove == null)
                    .FirstOrDefaultAsync();

                var ticketHistoryList = await _context.TicketHistories
                    .Where(x => x.TicketConcernId == transferTicketExist.TicketConcernId
                     && x.IsApprove == null && x.Request.Contains(TicketingConString.Approval))
                    .FirstOrDefaultAsync();

                //var selectTransferRequestId = transferApprover
                //    .FirstOrDefault(x => x.ApproverLevel == transferApprover.Min(x => x.ApproverLevel));

                if (transferApprover is not null)
                {

                    if (transferTicketExist.TicketApprover != command.Users
                      || !approverPermissionList.Any(x => x.Contains(command.Role)))
                        return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                    transferApprover.IsApprove = true;

                    var userApprovalId = await _context.ApproverTicketings
                        .Where(x => x.TransferTicketConcernId == transferApprover.TransferTicketConcernId)
                        .FirstOrDefaultAsync();

                    //var validateUserApprover = userApprovalId
                    //    .FirstOrDefault(x => x.ApproverLevel == transferApprover.ApproverLevel + 1);

                    await ApprovalTicketHistory(ticketHistoryList, userDetails, command, cancellationToken);

                    //if(userApprovalId is not null)
                    //{
                    //    await ApprovalTransferNotification(transferTicketExist,userDetails, userApprovalId, command,cancellationToken);
                    //}
                    //else
                    //{
                        await UpdateTransferTicket(transferTicketExist, userDetails, command, cancellationToken);
                    //}
                    
                    var channelUser = await _context.ChannelUsers.Include(x => x.Channel)
                        .Where(x => x.UserId == transferTicketExist.TransferTo && x.Channel.IsActive == true).FirstOrDefaultAsync();
                    var updateRequestConcern = new RequestConcern
                    {
                        Id = transferTicketExist.TicketConcern.RequestConcern.Id,
                        ChannelId = channelUser.ChannelId,
                        AssignTo = channelUser.UserId,
                        
                    };

                    await unitOfWork.RequestTicket.UpdateRequest(updateRequestConcern, cancellationToken);
                    {
                        var updateTicketConcern = new TicketConcern
                        {
                            Id = transferTicketExist.TicketConcern.Id,
                            UserId = transferTicketExist.TransferTo,
                            AssignTo = transferTicketExist.TransferTo,
                            ApprovedDateBy = command.Users,
                            //TransferAt = dateToday,
                            

                        };

                        await unitOfWork.RequestTicket.UpdateTicket(updateTicketConcern, cancellationToken);
                    }
                }
                else
                {
                    return Result.Failure(TransferTicketError.ApproverUnAuthorized());
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            private  Task ApprovalTicketHistory(TicketHistory ticketHistory, User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                //var ticketHistoryApproval = ticketHistory
                //    .FirstOrDefault(x => x.Approver_Level != null
                //    && x.Approver_Level == ticketHistory.Min(x => x.Approver_Level));

                ticketHistory.TransactedBy = command.Transacted_By;
                ticketHistory.TransactionDate = DateTime.Now;
                ticketHistory.Request = TicketingConString.Approve;
                ticketHistory.Status = $"{TicketingConString.TransferApprove} {user.Fullname}";
                ticketHistory.IsApprove = true;

                return Task.CompletedTask;
            }

            private async Task UpdateTransferTicket(TransferTicketConcern transferTicketConcern,User user ,ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                transferTicketConcern.TicketApprover = null;
                transferTicketConcern.IsTransfer = true;
                transferTicketConcern.TransferBy = transferTicketConcern.TicketConcern.UserId;
                transferTicketConcern.TransferAt = DateTime.Now;
                transferTicketConcern.TargetDate = command.Target_Date;
                transferTicketConcern.TransferTo = command.Transfer_To;
                transferTicketConcern.ApprovedBy = user.Fullname;

                var ticketConcernExist = await _context.TicketConcerns
                    .FirstOrDefaultAsync(x => x.Id == transferTicketConcern.TicketConcernId);

                ticketConcernExist.IsTransfer = true;
                ticketConcernExist.TransferAt = DateTime.Now;
                ticketConcernExist.TransferBy = transferTicketConcern.TicketConcern.UserId;
                ticketConcernExist.Remarks = transferTicketConcern.TransferRemarks;
                ticketConcernExist.UserId = command.Transfer_To;
                ticketConcernExist.TargetDate = command.Target_Date;
                

                await ApprovedTicketNotification(transferTicketConcern, user, command, cancellationToken);

            }

            private async Task ApprovedTicketNotification(TransferTicketConcern transferTicketConcern,User user, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {

                var addTransferByTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {transferTicketConcern.TicketConcernId} has transfer",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.AddedBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId ,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferByTransactionNotification);

                var addTransferToTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket concern number {transferTicketConcern.TicketConcernId} has transfer",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TransferTo.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = transferTicketConcern.TicketConcernId,

                };

                await _context.TicketTransactionNotifications.AddAsync(addTransferToTransactionNotification);

            }

            private async Task ApprovalTransferNotification(TransferTicketConcern transferTicketConcern, User user, ApproverTicketing approverTicketing, ApprovedTransferTicketCommand command, CancellationToken cancellationToken)
            {
                transferTicketConcern.TicketApprover = approverTicketing.UserId;   
                transferTicketConcern.TargetDate = command.Target_Date;
                transferTicketConcern.TransferTo = command.Transfer_To;

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {transferTicketConcern.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = approverTicketing.UserId.Value,
                    Modules = PathConString.Approval,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addNewTicketTransactionNotification);

                var addTicketApproveNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {transferTicketConcern.TicketConcernId} was approved by {user.Fullname}",
                    AddedBy = user.Id,
                    Created_At = DateTime.Now,
                    ReceiveBy = transferTicketConcern.TransferBy.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.ForTransfer,
                    PathId = transferTicketConcern.TicketConcernId

                };

                await _context.TicketTransactionNotifications.AddAsync(addTicketApproveNotification);

            }
        }
    }
}
