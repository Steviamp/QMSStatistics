using QMSStatistics.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace QMSStatistics.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IConfiguration _config;
        public StatisticsService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<Branch>> GetAllBranchesAsync()
        {
            const string sql = @"
                SELECT DISTINCT 
                OFFICE_NR AS OfficeNr,
                OFFICE_NAME AS OfficeName
                FROM CUSTOMER_QUEUE_INFO_DAILY
                ORDER BY OFFICE_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = await conn.QueryAsync<Branch>(sql);
            return result.ToList();
        }

        public async Task<List<BranchStat>> GetHistoricalStatsAsync(DateTime from, DateTime to, string? area = null, string? branch = null)
        {
            Console.WriteLine("RealStatisticsService CALLED with branch: " + branch);
            var sql = @"
                SELECT 
                    OFFICE_NR AS OfficeNr,
                    OFFICE_NAME AS OfficeName,
                    COUNT(*) AS Incoming,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS Unattended,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS Served,
                    0 AS Golden,
                    AVG(WAITING_TIME) AS AvgWaitSecs,
                    AVG(SERVICE_TIME) AS AvgServiceSecs,
                    AVG(WAITING_TIME + SERVICE_TIME) AS AvgCustomerSecs,
                    SUM(CASE WHEN WAITING_TIME <= 600 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveWaitPct,
                    SUM(CASE WHEN SERVICE_TIME <= 300 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveServicePct,
                    MAX(DATEDIFF(SECOND, FORWARDING_DATETIME, END_DATETIME)) AS MaxWaitSecs
                    FROM CUSTOMER_QUEUE_INFO_DAILY
                    WHERE TICKET_DATETIME BETWEEN @From AND @To
                    AND (@Branch IS NULL OR @Branch = '' OR OFFICE_NR = TRY_CAST(@Branch AS INT))
                    GROUP BY OFFICE_NR, OFFICE_NAME
                    ORDER BY OFFICE_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = await conn.QueryAsync<BranchStat>(sql, new { From = from, To = to, Branch = branch });
            return result.ToList();
        }

        public async Task<List<BranchStat>> GetStatsByAreaAsync(DateTime from, DateTime to)
        {
            const string sql = @"
                SELECT 
                    LEVEL2_NR AS OfficeNr,
                    LEVEL2_NAME AS OfficeName,
                    COUNT(*) AS Incoming,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS Unattended,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS Served,
                    0 AS Golden,
                    AVG(WAITING_TIME) AS AvgWaitSecs,
                    AVG(SERVICE_TIME) AS AvgServiceSecs,
                    AVG(WAITING_TIME + SERVICE_TIME) AS AvgCustomerSecs,
                    SUM(CASE WHEN WAITING_TIME <= 600 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveWaitPct,
                    SUM(CASE WHEN SERVICE_TIME <= 300 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveServicePct,
                    MAX(DATEDIFF(SECOND, FORWARDING_DATETIME, END_DATETIME)) AS MaxWaitSecs
                FROM CUSTOMER_QUEUE_INFO_DAILY
                WHERE TICKET_DATETIME BETWEEN @From AND @To
                GROUP BY LEVEL2_NR, LEVEL2_NAME
                ORDER BY LEVEL2_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = await conn.QueryAsync<BranchStat>(sql, new { From = from, To = to });
            return result.ToList();
        }

        public async Task<List<BranchStat>> GetStatsByCityAsync(DateTime from, DateTime to)
        {
            const string sql = @"
                SELECT 
                    LEVEL3_NR AS OfficeNr,
                    LEVEL3_NAME AS OfficeName,
                    COUNT(*) AS Incoming,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS Unattended,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS Served,
                    0 AS Golden,
                    AVG(WAITING_TIME) AS AvgWaitSecs,
                    AVG(SERVICE_TIME) AS AvgServiceSecs,
                    AVG(WAITING_TIME + SERVICE_TIME) AS AvgCustomerSecs,
                    SUM(CASE WHEN WAITING_TIME <= 600 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveWaitPct,
                    SUM(CASE WHEN SERVICE_TIME <= 300 THEN 1 ELSE 0 END) * 1.0 / COUNT(*) * 100 AS ObjectiveServicePct,
                    MAX(DATEDIFF(SECOND, FORWARDING_DATETIME, END_DATETIME)) AS MaxWaitSecs
                FROM CUSTOMER_QUEUE_INFO_DAILY
                WHERE TICKET_DATETIME BETWEEN @From AND @To
                GROUP BY LEVEL3_NR, LEVEL3_NAME
                ORDER BY LEVEL3_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var result = await conn.QueryAsync<BranchStat>(sql, new { From = from, To = to });
            return result.ToList();
        }
    }
}
