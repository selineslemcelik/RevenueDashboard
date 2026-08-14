namespace RevenueDashboard.Models.Dtos;

public class RevenueRecordDto
{
    public DateTime Date { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}