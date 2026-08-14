namespace RevenueDashboard.Models.Dtos;

public class CountryRevenueDto
{
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty; // flag-icons css kodu (tr, mx, es...)
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}