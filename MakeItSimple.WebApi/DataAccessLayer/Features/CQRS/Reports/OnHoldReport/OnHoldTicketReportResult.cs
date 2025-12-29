namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OnHoldReport
{
    public partial class OnHoldTicketReport
    {
        public record OnHoldTicketReportResult 
        {
            public int TicketConcernId { get; set; }
            public string Concerns {  get; set; }
            public string Reason { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool? IsHold { get; set; }
            public DateTime? Resume_At { get; set; }
            public DateTime? ApprovedDate { get; set; }
            public string ApprovedBy { get; set; }
            public int? ServiceProviderId { get; set; }
            public string ServiceProviderName { get; set;}
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public int? Year { get; set; }
            public int? Month { get; set; }
            

        }
    }
}
