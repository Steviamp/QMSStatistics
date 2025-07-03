using QMSStatistics.Models;

namespace QMSStatistics.Services
{
    public interface IStatisticsService
    {
        Task<List<Branch>> GetAllBranchesAsync();
        Task<List<BranchStat>> GetHistoricalStatsAsync(DateTime from, DateTime to, string? area = null, string? branch = null);
        Task<List<BranchStat>> GetStatsByCityAsync(DateTime from, DateTime to);
        Task<List<BranchStat>> GetStatsByAreaAsync(DateTime from, DateTime to);
    }
}
