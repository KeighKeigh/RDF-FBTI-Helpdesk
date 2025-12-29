namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.OpenReport
{
    public partial class OpenTicketReports
    {
        public record OpenTicketReportsResult
        {
            public int? Year {  get; set; }
            public int? Month { get; set; }
            public int? TicketConcernId { get; set; }
            public string Concern_Description { get; set; }
            public string Requestor_Name { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string Business_Unit_Code { get; set; }
            public string Business_Unit_Name { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }
            public string Category_Description { get; set; }
            public string SubCategory_Description { get; set; }
            public string Issue_Handler { get; set; }
            public string Channel_Name { get; set; }
            public DateTime? Target_Date { get; set; }
            public DateTime Created_At { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Updated_At { get; set; }
            public string Remarks { get; set; }
            public int Aging_Days { get; set; }
            public int? Personnel_Unit { get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public int? ServiceProvider { get; set; }
            public int? ChannelId { get; set; }
            public DateTime? StartDate { get; set; }
            public string AssigTo { get; set; }
            public Guid? AssignTo { get; set; }
            public string ConcernCategory { get; set; }
            public string RequestType { get; set; }
            public int? AgingDays { get; set; }
            public int? Rating { get; set; }
            public int? Backjobs { get; set; }
            public DateTime? DatePicked { get; set; }


        }
    }
}
