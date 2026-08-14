namespace RevenueDashboard.Models.Dtos;

public class MomentumDto
{
    public string Channel { get; set; } = string.Empty;
    public decimal CurrentRevenue { get; set; }
    public decimal PreviousRevenue { get; set; }
    public decimal ChangePercent { get; set; }
}