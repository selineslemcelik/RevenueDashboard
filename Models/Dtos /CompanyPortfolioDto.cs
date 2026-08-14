namespace RevenueDashboard.Models.Dtos;

public class CompanyPortfolioDto
{
    public string Company { get; set; } = string.Empty;
    public int ChannelCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public string TopContent { get; set; } = string.Empty;
    public decimal TopContentRevenue { get; set; }
}