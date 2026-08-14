using RevenueDashboard.Models.Dtos;
using RevenueDashboard.Repositories;

namespace RevenueDashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly IRevenueRepository _revenueRepository;

    public DashboardService(IRevenueRepository revenueRepository)
    {
        _revenueRepository = revenueRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        return await _revenueRepository.GetSummaryAsync();
    }
   public async Task<List<RevenueRecordDto>> GetRecordsAsync(RevenueFilterDto filter)
{
    ApplyPreset(filter);
    return await _revenueRepository.GetRecordsAsync(filter);
}
public async Task<List<string>> GetCompaniesAsync()
{
    return await _revenueRepository.GetCompaniesAsync();
}
public async Task<List<ChartPointDto>> GetMonthlyRevenueAsync()
{
    return await _revenueRepository.GetMonthlyRevenueAsync();
}
private void ApplyPreset(RevenueFilterDto filter)
{
    
    if (filter.Preset == DateRangePreset.None)
        return;

    var today = DateTime.Today;

    switch (filter.Preset)
    {
        case DateRangePreset.Last7Days:
            filter.StartDate = today.AddDays(-6);
            filter.EndDate = today;
            break;

        case DateRangePreset.Last28Days:
            filter.StartDate = today.AddDays(-27);
            filter.EndDate = today;
            break;

        case DateRangePreset.ThisWeek:
            filter.StartDate = StartOfWeek(today);
            filter.EndDate = today;
            break;

        case DateRangePreset.LastWeek:
            var lastWeekStart = StartOfWeek(today).AddDays(-7);
            filter.StartDate = lastWeekStart;
            filter.EndDate = lastWeekStart.AddDays(6);
            break;

        case DateRangePreset.ThisYear:
            filter.StartDate = new DateTime(today.Year, 1, 1);
            filter.EndDate = today;
            break;

        case DateRangePreset.LastYear:
            filter.StartDate = new DateTime(today.Year - 1, 1, 1);
            filter.EndDate = new DateTime(today.Year - 1, 12, 31);
            break;
    }
}

private static DateTime StartOfWeek(DateTime date)
{
    int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
    return date.AddDays(-diff);
}
public async Task<PeriodRevenueDto> GetPeriodRevenueAsync()
{
    return await _revenueRepository.GetPeriodRevenueAsync();
}
public async Task<List<ChartPointDto>> GetTopChannelsAsync()
{
    return await _revenueRepository.GetTopChannelsAsync();
}
public async Task<List<ChartPointDto>> GetRevenueByCompanyAsync()
{
    return await _revenueRepository.GetRevenueByCompanyAsync();
}
public async Task<List<ChartPointDto>> GetDailyRevenueAsync()
{
    return await _revenueRepository.GetDailyRevenueAsync();
}
public async Task<List<ChartPointDto>> GetTopDaysAsync()
    => await _revenueRepository.GetTopDaysAsync();

public async Task<List<ChartPointDto>> GetWeeklyRevenueAsync()
    => await _revenueRepository.GetWeeklyRevenueAsync();

public async Task<List<TopRecordDto>> GetTopRecordsAsync()
    => await _revenueRepository.GetTopRecordsAsync();
      public async Task<List<ChartPointDto>> GetBottomDaysAsync()
    => await _revenueRepository.GetBottomDaysAsync();
    public async Task<List<GrowthDto>> GetFastestGrowingChannelsAsync()
    => await _revenueRepository.GetFastestGrowingChannelsAsync();
    public async Task<List<CompanyTrendDto>> GetCompanyMonthlyTrendsAsync()
    => await _revenueRepository.GetCompanyMonthlyTrendsAsync();
    public async Task<List<ParetoPointDto>> GetChannelParetoAsync()
    => await _revenueRepository.GetChannelParetoAsync();
    public async Task<List<CompanyEfficiencyDto>> GetCompanyEfficiencyAsync()
    => await _revenueRepository.GetCompanyEfficiencyAsync();
    public async Task<List<CountryRevenueDto>> GetTopCountriesAsync()
    => await _revenueRepository.GetTopCountriesAsync();
    public async Task<List<PlatformRevenueDto>> GetPlatformRevenueAsync()
    => await _revenueRepository.GetPlatformRevenueAsync();
public async Task<List<CompanyPortfolioDto>> GetCompanyPortfolioAsync()
    => await _revenueRepository.GetCompanyPortfolioAsync();
    public async Task<List<ContinentRevenueDto>> GetContinentRevenueAsync()
    => await _revenueRepository.GetContinentRevenueAsync();
    public async Task<List<ChartPointDto>> GetHeatmapRevenueAsync()
    => await _revenueRepository.GetHeatmapRevenueAsync();

public async Task<List<CompanyPulseDto>> GetCompanyPulseAsync()
    => await _revenueRepository.GetCompanyPulseAsync();

public async Task<List<MomentumDto>> GetMomentumAsync()
    => await _revenueRepository.GetMomentumAsync();

public async Task<TodayGaugeDto> GetTodayGaugeAsync()
    => await _revenueRepository.GetTodayGaugeAsync();
    public async Task<List<CompanyMonthlyDto>> GetCompanyMonthlyAsync()
    => await _revenueRepository.GetCompanyMonthlyAsync();

public async Task<List<PlatformMonthlyDto>> GetPlatformMonthlyAsync()
    => await _revenueRepository.GetPlatformMonthlyAsync();
    public async Task<List<ChannelMonthlyDto>> GetTopChannelsMonthlyAsync()
    => await _revenueRepository.GetTopChannelsMonthlyAsync();
    public async Task<List<ContinentSeriesDto>> GetContinentTrendAsync()
    => await _revenueRepository.GetContinentTrendAsync();
    public async Task<List<GrowthDto>> GetTopMoversAsync()
    => await _revenueRepository.GetTopMoversAsync();
   
}
