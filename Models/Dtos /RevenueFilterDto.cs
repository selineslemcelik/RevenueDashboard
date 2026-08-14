namespace RevenueDashboard.Models.Dtos;

public class RevenueFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Company { get; set; }
    public string? ChannelName { get; set; }
    public DateRangePreset Preset { get; set; } = DateRangePreset.None;
}