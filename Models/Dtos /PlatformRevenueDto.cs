namespace RevenueDashboard.Models.Dtos;

public class PlatformRevenueDto
{
    public string Platform { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
}