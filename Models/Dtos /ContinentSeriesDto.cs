namespace RevenueDashboard.Models.Dtos;

public class ContinentSeriesDto
{
    public string Continent { get; set; } = string.Empty;
    public List<ChartPointDto> Points { get; set; } = new();
}