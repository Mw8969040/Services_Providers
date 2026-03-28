using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Dashboard.Queries;

namespace SmartPlatform.Application.Features.Dashboard.Handlers
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardDataDto>
    {
        private readonly IReadDbConnection _readDbConnection;
        private readonly ICacheService _cacheService;

        public GetDashboardStatsQueryHandler(IReadDbConnection readDbConnection, ICacheService cacheService)
        {
            _readDbConnection = readDbConnection;
            _cacheService = cacheService;
        }

        public async Task<DashboardDataDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = request.IsAdmin 
                ? "DashboardStats_Admin_Global" 
                : $"DashboardStats_{request.UserId}_Admin_False";

            var cachedData = await _cacheService.GetAsync<DashboardDataDto>(cacheKey);
            if (cachedData != null) return cachedData;

            // 1. تجميع كل الـ Queries في String واحد
            // ملاحظة: الـ @UserId هيمشي عليهم كلهم أوتوماتيك
            string providerFilter = request.IsAdmin ? "" : " AND s.ProviderId = @UserId ";

            var combinedSql = $@"
        -- 1. Get Top-level Stats
        SELECT 
            COUNT(sr.Id) as TotalRequests,
            SUM(CASE WHEN sr.RequestStatus = 0 THEN 1 ELSE 0 END) as PendingRequests,
            SUM(CASE WHEN sr.RequestStatus = 3 THEN 1 ELSE 0 END) as CompletedRequests,
            ISNULL(SUM(CASE WHEN sr.RequestStatus = 3 THEN sr.TotalPrice ELSE 0 END), 0) as TotalRevenue
        FROM ServiceRequests sr
        INNER JOIN Services s ON sr.ServiceId = s.Id
        WHERE sr.IsDeleted = 0 {providerFilter};

        -- 2. Get Revenue By Month
        SELECT 
            DATENAME(month, sr.RequestDate) as Month,
            MONTH(sr.RequestDate) as MonthNumber,
            ISNULL(SUM(sr.TotalPrice), 0) as Revenue,
            COUNT(sr.Id) as RequestCount
        FROM ServiceRequests sr
        INNER JOIN Services s ON sr.ServiceId = s.Id
        WHERE sr.IsDeleted = 0 
          AND sr.RequestStatus = 3 
          AND YEAR(sr.RequestDate) = YEAR(GETDATE())
          {providerFilter}
        GROUP BY DATENAME(month, sr.RequestDate), MONTH(sr.RequestDate)
        ORDER BY MONTH(sr.RequestDate);

        -- 3. Get Top Selling Services
        SELECT TOP 5
            s.Title as ServiceName,
            COUNT(sr.Id) as SalesCount
        FROM ServiceRequests sr
        INNER JOIN Services s ON sr.ServiceId = s.Id
        WHERE sr.IsDeleted = 0 
          AND sr.RequestStatus = 3
          {providerFilter}
        GROUP BY s.Title
        ORDER BY COUNT(sr.Id) DESC;
    ";

            // 2. تنفيذ الاستعلام مرة واحدة فقط باستخدام QueryMultipleAsync
            using (var multi = await _readDbConnection.QueryMultipleAsync(combinedSql, new { UserId = request.UserId }))
            {
                // قراءة النتائج بالترتيب اللي مكتوب في الـ SQL
                var stats = await multi.ReadFirstOrDefaultAsync<DashboardDataDto>();
                var data = stats ?? new DashboardDataDto();

                data.RevenueByMonth = (await multi.ReadAsync<MonthlyStatDto>()).ToList();
                data.TopServices = (await multi.ReadAsync<TopServiceDto>()).ToList();

                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromMinutes(5));

                return data;
            }
        }
    }
}
