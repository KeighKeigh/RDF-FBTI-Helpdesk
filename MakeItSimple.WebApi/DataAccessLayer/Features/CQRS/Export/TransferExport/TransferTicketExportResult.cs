namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.TransferExport
{
    public partial class TransferTicketExport
    {
        public record TransferTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public int? TicketConcernId { get; set; }
            public int? TransferTicketId { get; set; }
            public string Concern_Details { get; set; }
            public string Transfered_By { get; set; }
            public string Transfered_To { get; set; }
            public string Requested_Date { get; set; }
            public string New_Target_Date { get; set; }
            public string Previous_Target_Date { get; set; }
            public string Transfer_At { get; set; }
            public string Transfer_Remarks { get; set; }
            public string Remarks { get; set; }
            public string Modified_By { get; set; }
            public string Updated_At { get; set; }
            public string ApprovedBy { get; set; }
            public int? ChannnelId { get; set; }
            public string ChannnelName { get; set; }
            public int? ServiceProviderId { get; set; }
            public string ServiceProviderName { get; set;}
        }
    }
}
