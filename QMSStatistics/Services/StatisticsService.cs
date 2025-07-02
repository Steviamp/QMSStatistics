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
            var sql = @"
                SELECT 
                    OFFICE_NR AS OfficeNr,
                    OFFICE_NAME AS OfficeName,
                    COUNT(*) AS Incoming,
                    SUM(CASE WHEN LOSTTICKET = 1 THEN 1 ELSE 0 END) AS Unattended,
                    SUM(CASE WHEN LOSTTICKET = 0 THEN 1 ELSE 0 END) AS Served,
                    0 AS Golden, -- placeholder (δεν υπάρχει IsGolden)
                    AVG(WAITING_TIME) AS AvgWaitSecs,
                    AVG(SERVICE_TIME) AS AvgServiceSecs
                    FROM CUSTOMER_QUEUE_INFO_DAILY
                    WHERE TICKET_DATETIME BETWEEN @From AND @To
                    AND (@Branch IS NULL OR OFFICE_NR = @Branch)
                    GROUP BY OFFICE_NR, OFFICE_NAME";

            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var rawData = await conn.QueryAsync(sql, new { From = from, To = to, Branch = branch });

            var stats = rawData.Select(r => new BranchStat(
                r.OfficeNr,
                r.OfficeName,
                r.Incoming,
                r.Unattended,
                r.Served,
                r.Golden,
                TimeSpan.FromSeconds((int?)r.AvgWaitSecs ?? 0).ToString(@"hh\:mm\:ss"),
                TimeSpan.FromSeconds((int?)r.AvgServiceSecs ?? 0).ToString(@"hh\:mm\:ss")
            )).ToList();

            return stats;
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
            AVG(SERVICE_TIME) AS AvgServiceSecs
            FROM CUSTOMER_QUEUE_INFO_DAILY
            WHERE TICKET_DATETIME BETWEEN @From AND @To
            GROUP BY LEVEL2_NR, LEVEL2_NAME
            ORDER BY LEVEL2_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("QMS"));
            var rawData = await conn.QueryAsync(sql, new { From = from, To = to });

            return rawData.Select(r => new BranchStat(
                r.OfficeNr,
                r.OfficeName,
                r.Incoming,
                r.Unattended,
                r.Served,
                r.Golden,
                TimeSpan.FromSeconds((int?)r.AvgWaitSecs ?? 0).ToString(@"hh\:mm\:ss"),
                TimeSpan.FromSeconds((int?)r.AvgServiceSecs ?? 0).ToString(@"hh\:mm\:ss")
            )).ToList();
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
            AVG(SERVICE_TIME) AS AvgServiceSecs
        FROM CUSTOMER_QUEUE_INFO_DAILY
        WHERE TICKET_DATETIME BETWEEN @From AND @To
        GROUP BY LEVEL3_NR, LEVEL3_NAME
        ORDER BY LEVEL3_NR";

            using var conn = new SqlConnection(_config.GetConnectionString("QMS"));
            var rawData = await conn.QueryAsync(sql, new { From = from, To = to });

            return rawData.Select(r => new BranchStat(
                r.OfficeNr,
                r.OfficeName,
                r.Incoming,
                r.Unattended,
                r.Served,
                r.Golden,
                TimeSpan.FromSeconds((int?)r.AvgWaitSecs ?? 0).ToString(@"hh\:mm\:ss"),
                TimeSpan.FromSeconds((int?)r.AvgServiceSecs ?? 0).ToString(@"hh\:mm\:ss")
            )).ToList();
        }
    }
}
