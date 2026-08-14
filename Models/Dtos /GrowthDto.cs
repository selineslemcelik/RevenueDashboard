namespace RevenueDashboard.Models.Dtos;

public class GrowthDto
{
    public string Name { get; set; } = string.Empty;
    public decimal CurrentRevenue { get; set; }
    public decimal PreviousRevenue { get; set; }
    public decimal GrowthPercent { get; set; }
}