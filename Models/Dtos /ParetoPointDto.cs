namespace RevenueDashboard.Models.Dtos;

public class ParetoPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal CumulativePercent { get; set; }
}