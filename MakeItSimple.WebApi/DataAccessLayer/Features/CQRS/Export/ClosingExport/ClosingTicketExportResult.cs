using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {
        public record class ClosingTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string IssueHandler { get; set; }
            public int? Ticket_Number { get; set; }
            public string Description { get; set; }
            public DateTime? Target_Date_DateTime { get; set; }
            public DateTime? Actual_Date_DateTime { get; set; }
            public string Target_Date { get; set; }
            public string ClosedDate { get; set; }
            public int Varience { get; set; }
            public string Efficeincy { get; set; }
            public string Status { get; set; }
            public string ClosingStatus { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public int Aging_Days { get; set; }
            public int? ServiceProvider { get; set; }
            public string ServiceProviderName { get; set; }
            public string OpenDate { get; set; }
            public string ForClosedDate { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string Notes { get; set; }
            public string CreatedAt { get; set; }
            public string ConfirmedAt { get; set; }
            public string Requestor { get; set; }
            public string CategoryConcern { get; set; }

            public string Company_Code { get; set; }
            public string Company_Name { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public string Contractor { get; set; }
            public string Resolution { get; set; }
            public string Technicians { get; set; }
            public string DatePicked { get; set; }
            public string RequestType { get; set; }
            public int? Rating { get; set; }
            public string SLAPercentage { get; set; }
            public int? Backjobs { get; set; }

        }
    }
}
