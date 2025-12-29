using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequest.UpdateRequestConcern;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.CherryPicking
{
    public class TakeCherryPicking
    {

        public class TakeCherryPickingCommand : IRequest<Result>
        {
            public List<TicketConcernIdCherryList> ticketConcernIdCherryLists { get; set; }
            public class TicketConcernIdCherryList
            {

                public int? TicketConcernId { get; set; }
                public int? RequestConcernId { get; set; }
            }


            public DateTime? TargetDate { get; set; }
            public Guid? AssignTo { get; set; }
            public string Reason { get; set; }

        }

        public class Handler : IRequestHandler<TakeCherryPickingCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            //private readonly ICacheService cacheService;
            //private readonly IHubCaller hubCaller;
            private readonly MisDbContext context;

            public Handler(IUnitOfWork unitOfWork, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                //this.cacheService = cacheService;
                //this.hubCaller = hubCaller;
                this.context = context;

            }


            public async Task<Result> Handle(TakeCherryPickingCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Now;
                var ticketConcernId = new int(); //????kkkk
                var requestConcernId = new int();
                var ticketCategoryList = new List<int?>();
                var ticketSubCategoryList = new List<int?>();



                var approvedDate = DateTime.Now.AddDays(7);

                var handlerDetails = await unitOfWork.User
                        .UserExist(command.AssignTo);

                var handlerIds = await context.ApproverUsers.Where(x => x.UserId == command.AssignTo).FirstOrDefaultAsync();

                if (handlerIds == null)
                {
                    return Result.Failure(ClosingTicketError.NoApproverHasSetup());
                }


                foreach (var ticket in command.ticketConcernIdCherryLists)
                {

                    var requestConcernIdExist = await unitOfWork.RequestTicket
                .RequestConcernExist(ticket.RequestConcernId);


                    var ticketConcernIdExist = await unitOfWork.RequestTicket.TicketConcernExistAndRequestConcern(ticket.TicketConcernId, ticket.RequestConcernId);

                    if (ticketConcernIdExist is null)
                        return Result.Failure(TicketRequestError.ErrorInRequestConcern());

                    if (ticketConcernIdExist.IsApprove is true)
                        return Result.Failure(TicketRequestError.TicketAlreadyAssign());

                    if (requestConcernIdExist is not null)
                    {

                        var userDetails = await unitOfWork.User
                        .UserExist(ticketConcernIdExist.RequestorBy);

                        var requestorDetails = await unitOfWork.User
                                 .UserExist(ticketConcernIdExist.UserId);

                            var updateRequest = new RequestConcern
                            {
                                Id = requestConcernIdExist.Id,
                                ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                                ModifiedBy = command.AssignTo,
                                TargetDate = command.TargetDate.ToString() == "" ? null : command.TargetDate,
                                AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,
                                DatePicked = DateTime.Now,
                            };
                            await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequest, cancellationToken);

                            
                            var updateTicketConcern = new TicketConcern
                            {
                                Id = ticketConcernIdExist.Id,
                                TargetDate = command.TargetDate,
                                UserId = command.AssignTo,
                                IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                                IsAssigned = true,
                                ApprovedBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,
                                ApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,
                                ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                                AssignTo = command.AssignTo,
                                IsDateApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                                DateApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,
                                ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,
                                Reason = command.Reason,
                            };

                            await unitOfWork.RequestTicket.UpdateTicketConcerns(updateTicketConcern, cancellationToken);
                        
                        ticketConcernId = ticketConcernIdExist.Id; //kk
                        requestConcernId = requestConcernIdExist.Id;
                    }

                    if (command.AssignTo != null)
                    {


                        var addNewDateApproveConcern = new ApproverDate
                        {
                            TicketConcernId = ticketConcernIdExist.Id,
                            IsApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            TicketApprover = handlerIds.ApproverId,
                            AddedBy = ticketConcernIdExist.AssignTo,
                            ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.ApproverId : null,
                        };

                        await unitOfWork.RequestTicket.ApproveDateTicket(addNewDateApproveConcern, cancellationToken);

                        await unitOfWork.SaveChangesAsync(cancellationToken);

                        await unitOfWork.RequestTicket.UpdateAttachmentForCherryPicking(addNewDateApproveConcern, cancellationToken);

                        var addNewApprover = new ApproverTicketing
                        {
                            TicketConcernId = ticketConcernIdExist.Id,
                            ApproverDateId = addNewDateApproveConcern.Id,
                            UserId = handlerIds.ApproverId,
                            AddedBy = command.AssignTo,
                            CreatedAt = DateTime.Now,
                            Status = TicketingConString.ApprovalDate,
                            IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : null,
                        };

                        await unitOfWork.RequestTicket.CreateApproval(addNewApprover, cancellationToken);




                        var assignedTicketHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernIdExist.Id,
                            TransactedBy = command.AssignTo,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ConcernAssign,
                            Status = $"{TicketingConString.RequestAssign} {handlerDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(assignedTicketHistory, cancellationToken);


                        
                        //kk
                        if (command.TargetDate.Value.Date > approvedDate.Date)
                        {

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernIdExist.Id} has been assigned",
                                AddedBy = command.AssignTo.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = ticketConcernIdExist.RequestorBy.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.ForApproval,
                                PathId = ticketConcernIdExist.Id,

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);



                            var approveTicketDateHistory = new TicketHistory
                            {
                                TicketConcernId = ticketConcernIdExist.Id,
                                TransactedBy = handlerIds.ApproverId,
                                TransactionDate = null,
                                Request = TicketingConString.ForApprovalTicket,
                                Status = $"{TicketingConString.ForApprovalDate}"
                            };

                            await unitOfWork.RequestTicket.CreateTicketHistory(approveTicketDateHistory, cancellationToken);

                            var addNewTicketTransactionOngoing = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernIdExist.RequestConcernId} Target Date is being approved",
                                AddedBy = command.AssignTo.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = ticketConcernIdExist.RequestorBy.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.ForApproval,
                                PathId = ticketConcernIdExist.RequestConcernId.Value,


                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        }
                        else
                        {

                            //var addNewTicketTransactionNotifications = new TicketTransactionNotification
                            //{

                            //    Message = $"Ticket number {ticketConcernIdExist.Id}, Target Date is approved",
                            //    AddedBy = handlerIds.UserId,
                            //    Created_At = DateTime.Now,
                            //    ReceiveBy = requestConcernIdExist.UserId.Value,
                            //    Modules = PathConString.ConcernTickets,
                            //    Modules_Parameter = PathConString.Ongoing,
                            //    PathId = ticketConcernIdExist.Id

                            //};

                            //await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotifications, cancellationToken);

                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernIdExist.Id}, Target Date is approved",
                                AddedBy = handlerIds.UserId,
                                Created_At = DateTime.Now,
                                ReceiveBy = ticketConcernIdExist.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = ticketConcernIdExist.Id

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);

                            var addNewTicketTransactionOngoing = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {ticketConcernIdExist.Id} is now ongoing",
                                AddedBy = handlerIds.UserId,
                                Created_At = DateTime.Now,
                                ReceiveBy = requestConcernIdExist.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = ticketConcernIdExist.Id,


                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        }
                    }
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }
    }
}
