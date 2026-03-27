using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Dashboard.Queries;

namespace SmartPlatform.Application.Features.Dashboard.Handlers
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardDataDto>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetDashboardStatsQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<DashboardDataDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var data = new DashboardDataDto();
            
            // Base Where Clause for Provider Filtering
            string providerFilter = request.IsAdmin ? "" : " AND s.ProviderId = @UserId ";

            // 1. Get Top-level Stats
            var statsSql = $@"
                SELECT 
                    COUNT(sr.Id) as TotalRequests,
                    SUM(CASE WHEN sr.RequestStatus = 0 THEN 1 ELSE 0 END) as PendingRequests,
                    SUM(CASE WHEN sr.RequestStatus = 2 THEN 1 ELSE 0 END) as CompletedRequests,
                    ISNULL(SUM(CASE WHEN sr.RequestStatus = 2 THEN sr.TotalPrice ELSE 0 END), 0) as TotalRevenue
                FROM ServiceRequests sr
                INNER JOIN Services s ON sr.ServiceId = s.Id
                WHERE sr.IsDeleted = 0 {providerFilter}
            ";

            var stats = await _readDbConnection.QueryFirstOrDefaultAsync<DashboardDataDto>(statsSql, new { UserId = request.UserId });
            
            if (stats != null)
            {
                data.TotalRequests = stats.TotalRequests;
                data.PendingRequests = stats.PendingRequests;
                data.CompletedRequests = stats.CompletedRequests;
                data.TotalRevenue = stats.TotalRevenue;
            }

            // 2. Get Revenue By Month (Current Year)
            // SQL Server specific syntax for months
            var monthlySql = $@"
                SELECT 
                    DATENAME(month, sr.RequestDate) as Month,
                    MONTH(sr.RequestDate) as MonthNumber,
                    ISNULL(SUM(sr.TotalPrice), 0) as Revenue,
                    COUNT(sr.Id) as RequestCount
                FROM ServiceRequests sr
                INNER JOIN Services s ON sr.ServiceId = s.Id
                WHERE sr.IsDeleted = 0 
                  AND sr.RequestStatus = 2 -- Only Completed requests for revenue
                  AND YEAR(sr.RequestDate) = YEAR(GETDATE())
                  {providerFilter}
                GROUP BY DATENAME(month, sr.RequestDate), MONTH(sr.RequestDate)
                ORDER BY MONTH(sr.RequestDate)
            ";

            var monthlyData = await _readDbConnection.QueryAsync<MonthlyStatDto>(monthlySql, new { UserId = request.UserId });
            data.RevenueByMonth = monthlyData.ToList();

            // 3. Get Top Selling Services
            var topServicesSql = $@"
                SELECT TOP 5
                    s.Title as ServiceName,
                    COUNT(sr.Id) as SalesCount
                FROM ServiceRequests sr
                INNER JOIN Services s ON sr.ServiceId = s.Id
                WHERE sr.IsDeleted = 0 
                  AND sr.RequestStatus = 2
                  {providerFilter}
                GROUP BY s.Title
                ORDER BY COUNT(sr.Id) DESC
            ";

            var topServicesData = await _readDbConnection.QueryAsync<TopServiceDto>(topServicesSql, new { UserId = request.UserId });
            data.TopServices = topServicesData.ToList();

            return data;
        }
    }
}
