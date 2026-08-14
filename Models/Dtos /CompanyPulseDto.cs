namespace RevenueDashboard.Models.Dtos;

public class CompanyPulseDto
{
    public string Company { get; set; } = string.Empty;
    public decimal TodayRevenue { get; set; }
    public decimal YesterdayRevenue { get; set; }
    public decimal ChangePercent { get; set; }
    public List<decimal> Sparkline { get; set; } = new();
}