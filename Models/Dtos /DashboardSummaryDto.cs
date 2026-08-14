namespace RevenueDashboard.Models.Dtos;

public class DashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalRecords { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalChannels { get; set; }
}