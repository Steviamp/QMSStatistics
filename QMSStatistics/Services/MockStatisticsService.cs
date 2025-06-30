using QMSStatistics.Models;

namespace QMSStatistics.Services
{
    public class MockStatisticsService : IStatisticsService
    {
        private static readonly List<BranchStat> mockData = new()
    {
        new("201", "Sveta Nedelya", 253, 64, 189, 10, "00:31:13", "00:10:11"),
        new("206", "Nadezhda", 206, 6, 200, 5, "00:17:42", "00:09:29"),
        new("209", "A. Stamboliyski", 147, 22, 125, 8, "00:20:33", "00:11:56")
    };

        public Task<List<BranchStat>> GetHistoricalStatsAsync(DateTime from, DateTime to, string? area = null, string? branch = null)
        {
            return Task.FromResult(mockData); // placeholder logic
        }

        public Task<List<Branch>> GetAllBranchesAsync()
        {
            return Task.FromResult(new List<Branch>
    {
        new() { OfficeNr = "201", OfficeName = "Sveta Nedelya" },
        new() { OfficeNr = "206", OfficeName = "Nadezhda" },
        new() { OfficeNr = "209", OfficeName = "A. Stamboliyski" }
    });
        }
    }
}
