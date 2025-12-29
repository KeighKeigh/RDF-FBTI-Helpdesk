using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Pms_Transaction;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.OneRdf;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;

namespace MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work
{
    public interface IUnitOfWork
    {
        IUserRepository User {  get; }
        IUserRoleRepository UserRole { get; }

        ILocationRepository Location { get; }
        IChannelRepository Channel { get; }
        ICategoryRepository Category { get; }
        ISubCategoryRepository SubCategory { get; }
        IReceiverRepository Receiver { get; }
        IBusinessUnitRepository BusinessUnit { get; }
        IPendingRequestRepository PendingRequests { get; }


        IRequestTicketRepository RequestTicket { get; }

        IClosingRepository ClosingTicket { get; }

        IPmsFormRepository PmsForm { get; }
        IPmsQuestionaireModulesRepository PmsQuestionaireModules { get; }
        IPmsQuestionRepository PmsQuestion { get; }
        IPmsApproverRepository PmsApprover { get; }
        IPmsRepository Pms { get; }
        IApproverDateRepository ApproverDate { get; }

        Task RollBackTransaction();
        Task CommitTransaction();

        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);

    }
}