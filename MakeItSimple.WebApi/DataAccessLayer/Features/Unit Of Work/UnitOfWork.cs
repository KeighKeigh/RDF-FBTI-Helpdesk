using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.OneRdf;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Pms_Transaction;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.User_Management;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.OneRdf;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.User_Management;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository.Pms_Form;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;
using MakeItSimple.WebApi.Models.OneCharging;

namespace MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MisDbContext context;
        private readonly ContentType contentType;


        public UnitOfWork(MisDbContext context,ContentType contentType)
        {
            this.context = context;
            this.contentType = contentType;

            User = new UserRepository(context);
            UserRole = new UserRoleRepository(context);

            Location = new LocationRepository(context);
            Channel = new ChannelRepository(context);

            Category = new CategoryRepository(context);
            SubCategory = new SubCategoryRepository(context);
            Receiver = new ReceiverRepository(context);
            BusinessUnit = new BusinessUnitRepository(context);

            RequestTicket = new RequestTicketRepository(context, contentType);
            ClosingTicket = new ClosingTicketRepository(context);
            ApproverDate = new ApproverDateRepository(context);
            PendingRequests = new PendingRequestRepository(context);


            PmsForm = new PmsFormRepository(context);
            PmsQuestionaireModules = new PmsQuestionaireModuleRepository(context);
            PmsQuestion = new PmsQuestionRepository(context);
            PmsApprover = new PmsApproverRepository(context);
            Pms = new PmsRepository(context,contentType);

        }

        public IUserRepository User { get; private set; }
        public IUserRoleRepository UserRole {  get; private set; }

        public ILocationRepository Location {  get; private set; }
        public IChannelRepository Channel {  get; private set; }
        public ICategoryRepository Category {  get; private set; }
        public ISubCategoryRepository SubCategory {  get; private set; }
        public IReceiverRepository Receiver {  get; private set; }
        public IBusinessUnitRepository BusinessUnit { get; private set; }


        //Ticketing
        public IRequestTicketRepository RequestTicket { get;private set; }
        public IClosingRepository ClosingTicket { get; private set;}
        public IApproverDateRepository ApproverDate { get; private set; }

        //OneRdf
        public IPendingRequestRepository PendingRequests { get; private set; }

        //Pms Transaction

        public IPmsFormRepository PmsForm { get; private set; }
        public IPmsQuestionaireModulesRepository PmsQuestionaireModules { get; private set; }
        public IPmsQuestionRepository PmsQuestion { get; private set; }
        public IPmsApproverRepository PmsApprover { get; private set; }
        public IPmsRepository Pms { get; private set; }



        public async Task CommitTransaction()
        {
            using var transaction = await context.Database.BeginTransactionAsync();

            await transaction.CommitAsync();
        }

        public async Task RollBackTransaction()
        {
           using var transaction = await context.Database.BeginTransactionAsync();

            await transaction.RollbackAsync();
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}