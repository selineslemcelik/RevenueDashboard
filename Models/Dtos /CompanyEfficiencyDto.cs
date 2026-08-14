namespace RevenueDashboard.Models.Dtos;

public class CompanyEfficiencyDto
{
    public string Company { get; set; } = string.Empty;
    public decimal TotalRevenue { get; set; }
    public int ChannelCount { get; set; }
    public decimal RevenuePerChannel { get; set; }
}