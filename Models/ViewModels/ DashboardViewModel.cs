using RevenueDashboard.Models.Dtos;

namespace RevenueDashboard.Models.ViewModels;

public class DashboardViewModel
{
public DashboardSummaryDto Summary { get; set; } = new();
public List<ChartPointDto> MonthlyRevenue { get; set; } = new();
public PeriodRevenueDto PeriodRevenue { get; set; } = new();
public List<ChartPointDto> TopChannels { get; set; } = new();
public List<ChartPointDto> RevenueByCompany { get; set; } = new();
public List<ChartPointDto> DailyRevenue { get; set; } = new();
public List<ChartPointDto> TopDays { get; set; } = new();
public List<ChartPointDto> WeeklyRevenue { get; set; } = new();
public List<TopRecordDto> TopRecords { get; set; } = new();
public List<ChartPointDto> BottomDays { get; set; } = new();
public List<GrowthDto> FastestGrowingChannels { get; set; } = new();
public List<CompanyTrendDto> CompanyTrends { get; set; } = new();
public List<ParetoPointDto> ChannelPareto { get; set; } = new();
public List<CompanyEfficiencyDto> CompanyEfficiency { get; set; } = new();
public List<CountryRevenueDto> TopCountries { get; set; } = new();
public List<PlatformRevenueDto> PlatformRevenue { get; set; } = new();
public List<CompanyPortfolioDto> CompanyPortfolio { get; set; } = new();
public List<ContinentRevenueDto> ContinentRevenue { get; set; } = new();
public List<ChartPointDto> HeatmapRevenue { get; set; } = new();
public List<CompanyPulseDto> CompanyPulse { get; set; } = new();
public List<MomentumDto> Momentum { get; set; } = new();
public TodayGaugeDto TodayGauge { get; set; } = new();
public List<CompanyMonthlyDto> CompanyMonthly { get; set; } = new();
public List<PlatformMonthlyDto> PlatformMonthly { get; set; } = new();
public List<ChannelMonthlyDto> TopChannelsMonthly { get; set; } = new();
public List<ContinentSeriesDto> ContinentTrend { get; set; } = new();
public List<GrowthDto> TopMovers { get; set; } = new();
}
