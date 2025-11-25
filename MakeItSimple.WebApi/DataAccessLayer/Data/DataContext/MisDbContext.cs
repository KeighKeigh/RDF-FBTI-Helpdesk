using MakeItSimple.WebApi.DataAccessLayer.Data.Pms_Transaction;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ApproverSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ChannelSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CompanySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.DepartmentSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormCheckBoxSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormDropdownSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormQuestionSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.FormSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.LocationSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.OneChargingSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_One.ApproverUserSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_One.ServiceProviderSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Phase_Two.Pms_Form_Setup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.Pivot;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.QuestionCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ReceiverSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubCategorySetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubUnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Setup.UnitSetup;
using MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Data.UserManagement;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.OneCharging;
using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelUserSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.FormCheckBoxSetup;
using MakeItSimple.WebApi.Models.Setup.FormDropdownSetup;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MakeItSimple.WebApi.Models.Setup.FormsQuestionSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ContractorSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using MakeItSimple.WebApi.Models.Setup.Pivot;
using MakeItSimple.WebApi.Models.Setup.QuestionCategorySetup;
using MakeItSimple.WebApi.Models.Setup.ReceiverSetup;
using MakeItSimple.WebApi.Models.Setup.SubCategorySetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;
using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.DataContext
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        // Setup

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<AccountTitle> AccountTitles { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<SubUnit> SubUnits { get; set; }
        public virtual DbSet<SubUnitLocationPivot> SubUnitLocations { get; set; }

        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<ChannelUser> ChannelUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Approver> Approvers { get; set; }
        public virtual DbSet<ServiceProviders> ServiceProviders { get; set; }
        public virtual DbSet<ServiceProviderChannel> ServiceProviderChannels { get; set; }
        public virtual DbSet<OneChargingMIS> OneChargings { get; set; }
        public virtual DbSet<OneBusinessUnit> OneBusinessUnits { get; set; }
        public virtual DbSet<OneCompany> OneCompanies { get; set; }
        public virtual DbSet<OneDepartment> OneDepartments { get; set; }
        public virtual DbSet<OneLocation> OneLocations { get; set; }
        public virtual DbSet<OneSubUnit> OneSubUnits { get; set; }
        public virtual DbSet<OneUnit> OneUnits { get; set; }
        public virtual DbSet<ApproverUser> ApproverUsers { get; set; }
        public virtual DbSet<CategoryConcern> CategoryConcerns { get; set; }
        public virtual DbSet<CategoryConcernChannel> CategoryConcernChannels { get; set; }
        public virtual DbSet<Contractor> Contractors { get; set; }
        public virtual DbSet<ContractorChannel> ContractorChannels { get; set; }




        //Ticketing

        public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }
        public virtual DbSet<TicketConcern> TicketConcerns { get; set; }
        public virtual DbSet<TransferTicketConcern> TransferTicketConcerns { get; set; }
        public virtual DbSet<ClosingTicket> ClosingTickets { get; set; }
        public virtual DbSet<ApproverTicketing> ApproverTicketings { get; set; }
        public virtual DbSet<TicketHistory> TicketHistories { get; set; }
        public virtual DbSet<Receiver> Receivers { get; set; }
        public virtual DbSet<RequestConcern> RequestConcerns { get; set; }
        public virtual DbSet<TicketComment> TicketComments { get; set; }
        public virtual DbSet<TicketCommentView> TicketCommentViews { get; set; }
        public virtual DbSet<TicketTransactionNotification> TicketTransactionNotifications { get; set; }
        public virtual DbSet<TicketOnHold> TicketOnHolds { get; set; }
        public virtual DbSet<TicketCategory> TicketCategories { get; set; }
        public virtual DbSet<TicketSubCategory> TicketSubCategories {  get; set; } 
        public virtual DbSet<TicketTechnician> TicketTechnicians { get; set; }
        public virtual DbSet<ApproverDate> ApproverDates { get; set; }
        //Pms Setup

        public virtual DbSet<PmsForm> PmsForms { get; set; }
        public virtual DbSet<PmsQuestionaireModule> PmsQuestionaireModules { get; set; }
        public virtual DbSet<PmsQuestionaire> PmsQuestionaires { get; set; }
        public virtual DbSet<QuestionTransactionId> QuestionTransactionIds { get; set; }
        public virtual DbSet<PmsQuestionType> PmsQuestionTypes { get; set; }
        public virtual DbSet<PmsApprover> PmsApprovers { get; set; }

        public virtual DbSet<Pms> Pms { get; set; }
        public virtual DbSet<PmsDetail> PmsDetails { get; set; }
        public virtual DbSet<PmsApproval> PmsApprovals { get; set; }
        public virtual DbSet<PmsAttachment> PmsAttachments { get; set; }
        public virtual DbSet<PmsHistory> PmsHistories { get; set; }     
        public virtual DbSet<PmsTechnician> PmsTechnicians { get; set; }


        //Phase 3

        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<QuestionCategory> QuestionCategories { get; set; }
        public virtual DbSet<FormQuestion> FormQuestions { get; set; }
        public virtual DbSet<FormCheckBox> FormCheckBoxes { get; set; }
        public virtual DbSet<FormDropdown> FormDropdowns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new BusinessUnitConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTitleConfiguration());
            modelBuilder.ApplyConfiguration(new SubUnitConfiguration());
            modelBuilder.ApplyConfiguration(new UnitConfiguration());
            modelBuilder.ApplyConfiguration(new SubUnitLocationConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SubCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new TransferTicketConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverTicketingConfiguration());
            modelBuilder.ApplyConfiguration(new TicketHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ClosingTicketConfiguration());
            modelBuilder.ApplyConfiguration(new RequestConcernConfiguration());
            modelBuilder.ApplyConfiguration(new ReceiverConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentViewConfiguration());
            modelBuilder.ApplyConfiguration(new TicketTransactionNotificationConfiguration());
            modelBuilder.ApplyConfiguration(new TicketOnHoldConfiguration());
            modelBuilder.ApplyConfiguration(new TicketTechnicianConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceProviderConfiguration());
            modelBuilder.ApplyConfiguration(new ApproverUserConfiguration());
            //modelBuilder.ApplyConfiguration(new OneChargingMISConfiguration());

            //Phase 2

            modelBuilder.ApplyConfiguration(new PmsFormConfiguration());
            modelBuilder.ApplyConfiguration(new PmsQuestionaireModuleConfiguration());
            modelBuilder.ApplyConfiguration(new PmsQuestionaireConfiguration());
            modelBuilder.ApplyConfiguration(new PmsApproverConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionTransactionIdConfiguration());
            modelBuilder.ApplyConfiguration(new PmsQuestionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PmsConfiguration());
            modelBuilder.ApplyConfiguration(new PmsApprovalConfiguration());
            modelBuilder.ApplyConfiguration(new PmsHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new PmsAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new PmsTechnicianConfiguration());

            //Phase 3

            modelBuilder.ApplyConfiguration(new FormConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new FormQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new FormCheckBoxConfiguration());
            modelBuilder.ApplyConfiguration(new FormDropdownConfiguration());

        }
    }
}
