namespace RevenueDashboard.Models.Dtos;

public class TodayGaugeDto
{
    public decimal TodayRevenue { get; set; }
    public decimal Average30 { get; set; }
    public decimal PercentOfAverage { get; set; }
    public int StreakDays { get; set; }
}