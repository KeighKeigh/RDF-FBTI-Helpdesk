
namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.NewFolder
{
    public partial class DateApproval
    {
        public class DateApprovalResult
        {
            public int? ApproverDatesId { get; set; }
            public int? TicketConcernId { get; set; }
            public int? ChannelId { get; set; }
            public string Channel_Name { get; set; }
            public Guid? UserId { get; set; }
            public string Fullname { get; set; }
            public string Concern_Details { get; set; }

            public bool IsActive { get; set; }
            public string ConcernStatus { get; set; }
            public string Reason { get; set; }

            public List<GetApproveDateTicketCategory> GetApproveDateTicketCategories { get; set; }

            public class GetApproveDateTicketCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetApproveDateTicketSubCategory> GetApproveDateTicketSubCategories { get; set; }

            public class GetApproveDateTicketSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }


            public DateTime? ApprovedAt { get; set; }
            public string Reject_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Added_By { get; set; }
            public DateTime? Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime? DateNeeded { get; set; }
            public int? Approver_Level { get; set; }


            public List<DateApprovalAttachment> DateApprovalAttachments { get; set; }
            public class DateApprovalAttachment
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
    }
}
