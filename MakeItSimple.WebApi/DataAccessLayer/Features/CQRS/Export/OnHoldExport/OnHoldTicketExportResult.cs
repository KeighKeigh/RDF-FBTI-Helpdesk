namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.OnHoldExport
{
    public partial class OnHoldTicketExport
    {
        public record OnHoldTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public int TicketConcernId { get; set; }
            public string Concerns { get; set; }
            public string Reason { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool? IsHold { get; set; }
            public DateTime? Resume_At { get; set; }
            public DateTime? ApprovedAt { get; set; }
            public string ApprovedBy { get; set; }
            public int? ServiceProviderId { get; set; }
            public string ServiceProviderName { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public int? Year { get; set; }
            public int? Month { get; set; }
        }

    }
}
