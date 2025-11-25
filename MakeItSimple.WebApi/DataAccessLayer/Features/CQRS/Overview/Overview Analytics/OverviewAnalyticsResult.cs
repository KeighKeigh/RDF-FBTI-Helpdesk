namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Overview.Overview_Analytics
{
    public partial class OverviewAnalytics
    {
        public record OverviewAnalyticsResult
        {
            public string Department {  get; set; }
            public int TotalTicket { get; set; }
            public int TotalClosed { get; set; }
            public int TotalDelay { get; set; }
            public int CompletedTickets { get; set; }
            public double AvgResolutionTime { get; set; }
            public decimal CompletionRate { get; set; }
            public List<OverviewAnalyticsDetail> OverviewAnalyticsDetails { get; set; }
            public record OverviewAnalyticsDetail
            {
                public Guid Id { get; set; }
                public string FullName { get; set; }
                public int OpenTicketCount { get; set; }
                public int OnTimeTicketCount { get; set; }
                public int NumberOfTicket { get; set; }
                public decimal PercentageTicket { get; set; }
                public decimal NumberOfDelay { get; set; }
                public decimal DelayTicketPercentage { get; set; }

            }
        }


    }
}
