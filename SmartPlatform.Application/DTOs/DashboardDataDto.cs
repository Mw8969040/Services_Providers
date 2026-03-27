using System.Collections.Generic;

namespace SmartPlatform.Application.DTOs
{
    public class DashboardDataDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedRequests { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<MonthlyStatDto> RevenueByMonth { get; set; } = new();
        public List<TopServiceDto> TopServices { get; set; } = new();
    }

    public class MonthlyStatDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int RequestCount { get; set; }
    }

    public class TopServiceDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
    }
}
