using Microsoft.AspNetCore.Mvc;
using RevenueDashboard.Services;
using RevenueDashboard.Models.Dtos;
using RevenueDashboard.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;


namespace RevenueDashboard.Controllers;
[Authorize]

public class DashboardController : Controller
{
private readonly IDashboardService _dashboardService;

public DashboardController(IDashboardService dashboardService)
    {
_dashboardService = dashboardService;
    }

    [HttpGet]
public async Task<IActionResult> Summary()
    {
var summary = await _dashboardService.GetSummaryAsync();
return Json(summary);
    }
[HttpGet]
public async Task<IActionResult> Index()
{
var summary = await _dashboardService.GetSummaryAsync();

var monthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync();

var periodRevenue = await _dashboardService.GetPeriodRevenueAsync();

var topChannels = await _dashboardService.GetTopChannelsAsync();

var revenueByCompany = await _dashboardService.GetRevenueByCompanyAsync();

var dailyRevenue = await _dashboardService.GetDailyRevenueAsync();

var weeklyRevenue = await _dashboardService.GetWeeklyRevenueAsync();

var topDays = await _dashboardService.GetTopDaysAsync();

var topRecords = await _dashboardService.GetTopRecordsAsync();

var bottomDays = await _dashboardService.GetBottomDaysAsync();

var fastestGrowing = await _dashboardService.GetFastestGrowingChannelsAsync();

var companyTrends = await _dashboardService.GetCompanyMonthlyTrendsAsync();

var channelPareto = await _dashboardService.GetChannelParetoAsync();

var companyEfficiency = await _dashboardService.GetCompanyEfficiencyAsync();

var topCountries = await _dashboardService.GetTopCountriesAsync();


var platformRevenue = await _dashboardService.GetPlatformRevenueAsync();


var companyPortfolio = await _dashboardService.GetCompanyPortfolioAsync();

var continentRevenue = await _dashboardService.GetContinentRevenueAsync();
var heatmapRevenue = await _dashboardService.GetHeatmapRevenueAsync();
var companyPulse = await _dashboardService.GetCompanyPulseAsync();
var momentum = await _dashboardService.GetMomentumAsync();
var todayGauge = await _dashboardService.GetTodayGaugeAsync();
var companyMonthly = await _dashboardService.GetCompanyMonthlyAsync();
var platformMonthly = await _dashboardService.GetPlatformMonthlyAsync();
var topChannelsMonthly = await _dashboardService.GetTopChannelsMonthlyAsync();
var continentTrend = await _dashboardService.GetContinentTrendAsync();
var topMovers = await _dashboardService.GetTopMoversAsync();

var viewModel = new DashboardViewModel

    {
Summary = summary,
MonthlyRevenue = monthlyRevenue,
PeriodRevenue = periodRevenue,
TopChannels = topChannels,
RevenueByCompany = revenueByCompany,
DailyRevenue = dailyRevenue,
WeeklyRevenue = weeklyRevenue,
TopDays = topDays,
BottomDays = bottomDays,
TopRecords = topRecords,
FastestGrowingChannels = fastestGrowing,
CompanyTrends = companyTrends,
ChannelPareto = channelPareto,
CompanyEfficiency = companyEfficiency,
TopCountries = topCountries,
PlatformRevenue = platformRevenue,
CompanyPortfolio = companyPortfolio,
ContinentRevenue = continentRevenue,
HeatmapRevenue = heatmapRevenue,
CompanyPulse = companyPulse,
Momentum = momentum,
TodayGauge = todayGauge,
CompanyMonthly = companyMonthly,
PlatformMonthly = platformMonthly,
TopChannelsMonthly = topChannelsMonthly,
ContinentTrend = continentTrend,
TopMovers = topMovers,
    };

return View(viewModel);
}
}
