namespace RevenueDashboard.Models.Dtos;

public class PlatformMonthlyDto
{
    public string Platform { get; set; } = string.Empty;

    public decimal ThisMonthRevenue { get; set; }

    public decimal LastMonthRevenue { get; set; }

    public decimal ChangePercent { get; set; }

    public string TopChannel { get; set; } = string.Empty;

    public decimal TopChannelRevenue { get; set; }

     public List<decimal> Sparkline12Month { get; set; } = new();
}