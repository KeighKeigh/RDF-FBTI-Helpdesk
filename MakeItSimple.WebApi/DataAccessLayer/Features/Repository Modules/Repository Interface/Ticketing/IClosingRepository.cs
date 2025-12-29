using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing
{
    public interface IClosingRepository
    {
        Task<ClosingTicket>ClosingTicketExist(int? id);
        Task<ApproverTicketing> ApproverThatNotNullByClosingTicket(int? id);
        Task<List<ApproverTicketing>> ApproverThatNullByClosingTicketList(int? id);
        Task<ApproverTicketing> ApproverByClosingTicketList(int? id);
        Task<ApproverTicketing> ApproverByMinLevel(int? id);
        Task<ApproverTicketing> ApproverPlusOne(int? id);
        Task<TicketTechnician> TicketTechnicianExist(int? id);  

        Task<List<Approver>> ApproverBySubUnitList(int? id);

        Task CreateClosingTicket(ClosingTicket closingTicket,CancellationToken cancellationToken);
        Task CreateApproval(ApproverTicketing approverTicketing,CancellationToken cancellationToken);
        Task CreateTicketTechnician(TicketTechnician ticketTechnician, CancellationToken cancellationToken);

        Task RemoveTicketTechnician(int id, List<int> ticketTechnicianId, CancellationToken cancellationToken);
        Task RemoveClosingApprover(int? id);
        Task RemoveClosingTicket(int? id);
        Task CancelClosingTicket(int? id);
        Task RejectClosingTicket(ClosingTicket closingTicket);
        Task ReturnClosingTicket(int? id, string status, string remarks);


        Task UpdateClosingTicket(ClosingTicket closingTicket,CancellationToken cancellationToken);
        Task ApprovedApproval(int? id);
        Task ApprovedClosingTicket(ClosingTicket closingTicket, CancellationToken cancellationToken);

        //Task AddForClosingDate(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task ApprovedTicketConcernByClosing(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task ApprovedRequestConcernByClosing(RequestConcern requestConcern, CancellationToken cancellation);
        Task ConfirmClosingTicket(int? id);
        Task ConfirmTicketHistory(int? id);
        Task RequestorConfirmation(int? id, Guid? requestor);



        Task NextApproverUser(int? id, Guid? userId);
        Task ForClosingTicket(int? id);
    }
}
