namespace RevenueDashboard.Models.Dtos;

public class TopRecordDto
{
    public DateTime Date { get; set; }
    public string Company { get; set; } = string.Empty;
    public string ChannelName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}