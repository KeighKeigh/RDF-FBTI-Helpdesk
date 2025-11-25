using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using NuGet.Packaging.Signing;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing
{
    public interface IRequestTicketRepository
    {

        Task CreateRequestConcern(RequestConcern requestConcern,CancellationToken cancellationToken);
        Task CreateTicketConcern(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task CreateTicketHistory(TicketHistory ticketHistory, CancellationToken cancellationToken);
        Task CreateTicketNotification(TicketTransactionNotification ticketNotification, CancellationToken cancellationToken);
        Task CreateTicketApproval(ApproverTicketing approverTicketing, CancellationToken cancellationToken);
        Task CreateTicketCategory(TicketCategory ticketCategory, CancellationToken cancellationToken);
        Task CreateTicketSubCategory(TicketSubCategory ticketSubCategory, CancellationToken cancellationToken);
        Task CreateTicketAttachment(TicketAttachment ticketAttachment, CancellationToken cancellationToken);

        Task<List<Approver>> ApproverBySubUnitList(int? id);
        Task ApproveDateTicket(ApproverDate approver, CancellationToken cancellationToken);
        Task CreateApproval(ApproverTicketing approverTicketing, CancellationToken cancellationToken);
        Task<int>PossibleRequestId();
        Task<int> PossibleTicketId();

        Task UpdateTicket(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task UpdateRequest(RequestConcern requestConcern, CancellationToken cancellationToken);
        Task UpdateTicketConcerns(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task UpdateRequestConcern(RequestConcern requestConcern, CancellationToken cancellationToken);
        Task UpdateTicketConcernss(TicketConcern ticketConcern, CancellationToken cancellationToken);
        Task UpdateRequestConcerns(RequestConcern requestConcern, CancellationToken cancellationToken);
        Task UpdateTicketConcern(TicketConcern ticketConcern, CancellationToken cancellationToken);

        Task UpdateTicketAttachment(TicketAttachment ticketAttachment, CancellationToken cancellationToken);

        Task UpdateTicketHistory(TicketHistory ticketHistory,CancellationToken cancellationToken);

        Task ApproveOpenTicket(TicketConcern ticketConcern, CancellationToken cancellationToken);
       

        Task<bool> UpdateAttachmentForCherryPicking(ApproverDate appproverDate , CancellationToken cancellationToken); 
        Task RemoveTicketCategory(int id, List<int?> categoryId, CancellationToken cancellationToken);
        Task RemoveTicketSubCategory(int id, List<int?> subCategoryId, CancellationToken cancellationToken);
        Task RemoveTicketHistory(int? id);
        Task CancelledTicketConcern(int? id);
        Task CancelledRequestConcern(int? id);
        Task CancelledTicketAttachment(int? id);


        Task<RequestConcern> RequestConcernExist(int? id);

        Task<TicketConcern> TicketConcernExistByRequestConcernId(int? id);
        Task<TicketConcern> TicketConcernExist(int? id);
        Task<TicketConcern> TicketConcernExistAndRequestConcern(int? id, int? reqId);

        //Task<TicketConcern> TicketConcernExistByRequestConcern(int? id);
        Task<TicketConcern> TicketConcernByRequest(int? id);
        Task<TicketCategory> TicketCategoryExist(int? id);
        Task<TicketCategory> TicketCategoryExist(int? id, int? requestConcernId);
        Task<TicketSubCategory> TicketSubCategoryExist(int? id, int? requestConcernId);
        Task<TicketSubCategory> TicketSubCategoryExist(int? id);

        Task<TicketAttachment> TicketAttachmentExist(int? id);
        Task<List<TicketHistory>> TicketHistoryByForApprovalList(int? id);
        Task<TicketHistory> TicketHistoryMinByForApproval(int? id);

    }
}
