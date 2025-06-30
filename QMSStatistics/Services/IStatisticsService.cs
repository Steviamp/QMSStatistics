using QMSStatistics.Models;

namespace QMSStatistics.Services
{
    public interface IStatisticsService
    {
        Task<List<BranchStat>> GetHistoricalStatsAsync(DateTime from, DateTime to, string? area = null, string? branch = null);
        Task<List<Branch>> GetAllBranchesAsync();
    }
}
