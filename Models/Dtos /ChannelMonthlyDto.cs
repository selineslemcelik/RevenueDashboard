namespace RevenueDashboard.Models.Dtos;

public class ChannelMonthlyDto
{
    public string ChannelName { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public decimal ThisMonthRevenue { get; set; }

    public decimal LastMonthRevenue { get; set; }

    public decimal ChangePercent { get; set; }
}