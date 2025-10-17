namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.TransferReport
{
    public partial class TransferTicketReports
    {
        public record TransferTicketReportsResult
        {
            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public string Concern_Details { get; set; }
            public string Transfered_By { get; set; }
            public string Transfered_To { get; set; }
            public DateTime? Requested_Date { get; set; }
            public DateTime? New_Target_Date { get; set; }
            public DateTime? Previous_Target_Date { get; set; }
            public DateTime? Approved_At { get; set; }
            public string Transfer_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string ApprovedBy { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public int? ServiceProviderId { get; set; }
            public string ServiceProviderName { get; set;}


        }
    }
}
