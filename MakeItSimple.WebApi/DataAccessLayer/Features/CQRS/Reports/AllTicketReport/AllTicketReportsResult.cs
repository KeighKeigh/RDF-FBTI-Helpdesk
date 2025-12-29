namespace MakeItSimple.WebApi.DataAccessLayer.Features.Reports.AllTicketReport
{
    public partial class AllTicketReports
    {
        public record AllTicketReportsResult
        {
            public string TicketConcernId { get; set; }
            public string Request_Type { get; set; }
            public string BackJobId { get; set; }
            public string Requestor_Name { get; set; }
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
            public int? Personnel_Unit {  get; set; }
            public Guid? Personnel_Id { get; set; }
            public string Personnel { get; set; }
            public string Concerns { get; set; }
            public string Channel_Name { get; set; }
            public string TicketCategoryDescriptions { get; set; }
            public string TicketSubCategoryDescriptions { get; set; }
            public string Date_Needed { get; set; }
            public string Contact_Number { get; set; }
            public string Notes { get; set; }
            public string Transaction_Date { get; set; }
            public string Target_Date { get; set; }
            public string Ticket_Status { get; set; }
            public string Remarks { get; set; }
            public int? Aging_Days {  get; set; }    
            public int? Rating { get; set; }
            public int? ChannelId { get; set; }
            public int? ServiceProvider { get; set; }
            public string ServiceProviderName { get; set; }
            public string StartDate { get; set; }
            public string ClosedDate { get; set; }
            public string ForClosedDate { get; set; }
            public string AssignTo { get; set; }
            public string ClosingStatus { get; set; }
            public string Technician1 { get; set; }
            public string Technician2 { get; set; }
            public string Technician3 { get; set; }
            public string Resolution { get; set; }
            public string CreatedTime { get; set; }
            public string CompletedTime { get; set;}
            public string Severity { get; set; }
            public string Technicians { get; set; }
            public string CategoryConcern { get; set; }
            public string Contractor { get; set; }

            public int? Year { get; set; }
            public int? Month { get; set; }
            public int? SLAGrading { get; set; }
            public string SLAPercentage { get; set; }
            public int? Backjobs { get; set; }
            public string DatePicked { get; set; }



        }

    }
}
