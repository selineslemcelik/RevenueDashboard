namespace RevenueDashboard.Models.Dtos;

public class CompanyTrendDto
{
    public string Company { get; set; } = string.Empty;
    public List<string> Months { get; set; } = new();      // ["2025-08", "2025-09", ...]
    public List<decimal> Values { get; set; } = new();      // her aya karşılık gelen gelir
}