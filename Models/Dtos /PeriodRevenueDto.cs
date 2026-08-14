namespace RevenueDashboard.Models.Dtos;

public class PeriodRevenueDto
{
    public decimal Today { get; set; }
    public decimal Yesterday { get; set; }
    public decimal ThisWeek { get; set; }
    public decimal LastWeek { get; set; }
    public decimal ThisMonth { get; set; }
    public decimal LastMonth { get; set; }
    public decimal ThisYear { get; set; }
    public decimal LastYear { get; set; }
}