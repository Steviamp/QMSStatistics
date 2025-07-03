namespace QMSStatistics.Models
{
    public class BranchStat
    {
        public string OfficeNr { get; set; } = string.Empty;
        public string OfficeName { get; set; } = string.Empty;
        public int Incoming { get; set; }
        public int Unattended { get; set; }
        public int Served { get; set; }
        public int Golden { get; set; }
        public double AvgWaitSecs { get; set; }
        public double AvgServiceSecs { get; set; }
        public double AvgCustomerSecs { get; set; }
        public double ObjectiveWaitPct { get; set; }
        public double ObjectiveServicePct { get; set; }
        public int MaxWaitSecs { get; set; }
    }

}
