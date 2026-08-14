using RevenueDashboard.Models.Dtos;

namespace RevenueDashboard.Repositories;

public interface IRevenueRepository
{
Task<DashboardSummaryDto> GetSummaryAsync();
Task<List<RevenueRecordDto>> GetRecordsAsync(RevenueFilterDto filter);
Task<List<string>> GetCompaniesAsync();
Task<List<ChartPointDto>> GetMonthlyRevenueAsync();
Task<PeriodRevenueDto> GetPeriodRevenueAsync();
Task<List<ChartPointDto>> GetTopChannelsAsync();
Task<List<ChartPointDto>> GetRevenueByCompanyAsync();
Task<List<ChartPointDto>> GetDailyRevenueAsync();
Task<List<ChartPointDto>> GetTopDaysAsync();
Task<List<ChartPointDto>> GetWeeklyRevenueAsync();
Task<List<TopRecordDto>> GetTopRecordsAsync();
Task<List<ChartPointDto>> GetBottomDaysAsync();
Task<List<GrowthDto>> GetFastestGrowingChannelsAsync();
Task<List<CompanyTrendDto>> GetCompanyMonthlyTrendsAsync();
Task<List<ParetoPointDto>> GetChannelParetoAsync();
Task<List<CompanyEfficiencyDto>> GetCompanyEfficiencyAsync();
Task<List<CountryRevenueDto>> GetTopCountriesAsync();
Task<List<PlatformRevenueDto>> GetPlatformRevenueAsync();
Task<List<CompanyPortfolioDto>> GetCompanyPortfolioAsync();
Task<List<ContinentRevenueDto>> GetContinentRevenueAsync();
Task<List<ChartPointDto>> GetHeatmapRevenueAsync();
Task<List<CompanyPulseDto>> GetCompanyPulseAsync();
Task<List<MomentumDto>> GetMomentumAsync();
Task<TodayGaugeDto> GetTodayGaugeAsync();
Task<List<CompanyMonthlyDto>> GetCompanyMonthlyAsync();
Task<List<PlatformMonthlyDto>> GetPlatformMonthlyAsync();
Task<List<ChannelMonthlyDto>> GetTopChannelsMonthlyAsync();
Task<List<ContinentSeriesDto>> GetContinentTrendAsync();
Task<List<GrowthDto>> GetTopMoversAsync();

}
