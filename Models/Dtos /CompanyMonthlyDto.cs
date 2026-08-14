namespace RevenueDashboard.Models.Dtos;

public class CompanyMonthlyDto
{
    public string Company { get; set; } = string.Empty;
    public decimal ThisMonthRevenue { get; set; }
    public decimal LastMonthRevenue { get; set; }
    public decimal ChangePercent { get; set; }
    public List<decimal> Sparkline6Month { get; set; } = new();
}