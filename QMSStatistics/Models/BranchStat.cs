namespace QMSStatistics.Models
{
    public record BranchStat(
    string OfficeNr,
    string OfficeName,
    int Incoming,
    int Unattended,
    int Served,
    int Golden,
    string AvgWait,
    string AvgService
);
}
