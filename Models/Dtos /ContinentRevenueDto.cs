namespace RevenueDashboard.Models.Dtos;

public class ContinentRevenueDto
{
    public string Continent { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}