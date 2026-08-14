using Npgsql;
using RevenueDashboard.Infrastructure;
using RevenueDashboard.Models.Dtos;
using System.Text;

namespace RevenueDashboard.Repositories;

public class RevenueRepository : IRevenueRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RevenueRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
   public async Task<DashboardSummaryDto> GetSummaryAsync()
{
    const string sql = @"
        SELECT
            COALESCE(SUM(revenue), 0)::numeric   AS total_revenue,
            COUNT(*)::int                         AS total_records,
            COUNT(DISTINCT company)::int          AS total_companies,
            COUNT(DISTINCT channel_name)::int     AS total_channels
        FROM revenues;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    await reader.ReadAsync();

    var summary = new DashboardSummaryDto
    {
        TotalRevenue = reader.GetDecimal(0),
        TotalRecords = reader.GetInt32(1),
        TotalCompanies = reader.GetInt32(2),
        TotalChannels = reader.GetInt32(3)
    };

    return summary;
}
public async Task<List<RevenueRecordDto>> GetRecordsAsync(RevenueFilterDto filter)
{
    var sql = new StringBuilder(@"
        SELECT date, channel_name, company, revenue
        FROM revenues
        WHERE 1 = 1");

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand();
    command.Connection = connection;

    if (filter.StartDate.HasValue)
    {
        sql.Append(" AND date >= @startDate");
        command.Parameters.AddWithValue("startDate", filter.StartDate.Value);
    }
    if (filter.EndDate.HasValue)
    {
        sql.Append(" AND date <= @endDate");
        command.Parameters.AddWithValue("endDate", filter.EndDate.Value);
    }
    if (!string.IsNullOrWhiteSpace(filter.Company))
    {
        sql.Append(" AND company = @company");
        command.Parameters.AddWithValue("company", filter.Company);
    }
    if (!string.IsNullOrWhiteSpace(filter.ChannelName))
    {
        sql.Append(" AND channel_name = @channelName");
        command.Parameters.AddWithValue("channelName", filter.ChannelName);
    }

    sql.Append(" ORDER BY date DESC LIMIT 50;");
    command.CommandText = sql.ToString();

    var records = new List<RevenueRecordDto>();

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        records.Add(new RevenueRecordDto
        {
            Date = reader.GetDateTime(0),
            ChannelName = reader.GetString(1),
            Company = reader.GetString(2),
            Revenue = reader.GetDecimal(3)
        });
    }

    return records;
}
public async Task<List<string>> GetCompaniesAsync()
{
    const string sql = "SELECT DISTINCT company FROM revenues ORDER BY company;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var companies = new List<string>();
    while (await reader.ReadAsync())
    {
        companies.Add(reader.GetString(0));
    }

    return companies;
}
public async Task<List<ChartPointDto>> GetMonthlyRevenueAsync()
{
    const string sql = @"
        SELECT
            TO_CHAR(date, 'YYYY-MM') AS month,
            COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE date >= date_trunc('month', CURRENT_DATE) - INTERVAL '11 months'
        GROUP BY TO_CHAR(date, 'YYYY-MM')
        ORDER BY month;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new ChartPointDto
        {
            Label = reader.GetString(0),
            Value = reader.GetDecimal(1)
        });
    }

    return result;
}
public async Task<PeriodRevenueDto> GetPeriodRevenueAsync()
{
    const string sql = @"
        WITH ref AS (SELECT COALESCE(MAX(date), CURRENT_DATE) AS ref_date FROM revenues),
        calc AS (
            SELECT
                ref_date,
                (ref_date - date_trunc('month', ref_date)::date) AS month_offset,
                (ref_date - date_trunc('week',  ref_date)::date) AS week_offset,
                (ref_date - date_trunc('year',  ref_date)::date) AS year_offset
            FROM ref
        )
        SELECT
            -- Bugün / Dün
            COALESCE(SUM(revenue) FILTER (WHERE date = (SELECT ref_date FROM calc)), 0)::numeric,
            COALESCE(SUM(revenue) FILTER (WHERE date = (SELECT ref_date FROM calc) - INTERVAL '1 day'), 0)::numeric,
            -- Bu hafta (hafta başı → ref) / Geçen hafta (aynı gün ofsetine kadar)
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('week',(SELECT ref_date FROM calc))
                     AND date <= (SELECT ref_date FROM calc)), 0)::numeric,
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('week',(SELECT ref_date FROM calc)) - INTERVAL '1 week'
                     AND date <= date_trunc('week',(SELECT ref_date FROM calc)) - INTERVAL '1 week' + ((SELECT week_offset FROM calc) || ' days')::interval), 0)::numeric,
            -- Bu ay (ay başı → ref) / Geçen ay (aynı gün ofsetine kadar)
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('month',(SELECT ref_date FROM calc))
                     AND date <= (SELECT ref_date FROM calc)), 0)::numeric,
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('month',(SELECT ref_date FROM calc)) - INTERVAL '1 month'
                     AND date <= date_trunc('month',(SELECT ref_date FROM calc)) - INTERVAL '1 month' + ((SELECT month_offset FROM calc) || ' days')::interval), 0)::numeric,
            -- Bu yıl (yıl başı → ref) / Geçen yıl (aynı gün ofsetine kadar)
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('year',(SELECT ref_date FROM calc))
                     AND date <= (SELECT ref_date FROM calc)), 0)::numeric,
            COALESCE(SUM(revenue) FILTER (WHERE date >= date_trunc('year',(SELECT ref_date FROM calc)) - INTERVAL '1 year'
                     AND date <= date_trunc('year',(SELECT ref_date FROM calc)) - INTERVAL '1 year' + ((SELECT year_offset FROM calc) || ' days')::interval), 0)::numeric
        FROM revenues;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    await reader.ReadAsync();

    return new PeriodRevenueDto
    {
        Today = reader.GetDecimal(0),
        Yesterday = reader.GetDecimal(1),
        ThisWeek = reader.GetDecimal(2),
        LastWeek = reader.GetDecimal(3),
        ThisMonth = reader.GetDecimal(4),
        LastMonth = reader.GetDecimal(5),
        ThisYear = reader.GetDecimal(6),
        LastYear = reader.GetDecimal(7)
    };
}
public async Task<List<ChartPointDto>> GetTopChannelsAsync()
{
    const string sql = @"
        SELECT
            channel_name AS label,
            COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        GROUP BY channel_name
        ORDER BY total DESC
        LIMIT 20;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new ChartPointDto
        {
            Label = reader.GetString(0),
            Value = reader.GetDecimal(1)
        });
    }

    return result;
}
public async Task<List<ChartPointDto>> GetRevenueByCompanyAsync()
{
    const string sql = @"
        SELECT
            company AS label,
            COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        GROUP BY company
        ORDER BY total DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new ChartPointDto
        {
            Label = reader.GetString(0),
            Value = reader.GetDecimal(1)
        });
    }

    return result;
}
public async Task<List<ChartPointDto>> GetDailyRevenueAsync()
{
    const string sql = @"
        SELECT
            TO_CHAR(date, 'YYYY-MM-DD') AS day,
            COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE date >= CURRENT_DATE - INTERVAL '29 days'
        GROUP BY TO_CHAR(date, 'YYYY-MM-DD')
        ORDER BY day;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new ChartPointDto
        {
            Label = reader.GetString(0),
            Value = reader.GetDecimal(1)
        });
    }

    return result;
}
public async Task<List<ChartPointDto>> GetTopDaysAsync()
{
    const string sql = @"
        SELECT TO_CHAR(date, 'YYYY-MM-DD') AS label,
               COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        GROUP BY date
        ORDER BY total DESC
        LIMIT 5;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
        result.Add(new ChartPointDto { Label = reader.GetString(0), Value = reader.GetDecimal(1) });
    return result;
}

public async Task<List<ChartPointDto>> GetWeeklyRevenueAsync()
{
    const string sql = @"
        SELECT TO_CHAR(date_trunc('week', date), 'YYYY-MM-DD') AS label,
               COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE date >= date_trunc('week', CURRENT_DATE) - INTERVAL '7 weeks'
        GROUP BY date_trunc('week', date)
        ORDER BY label;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
        result.Add(new ChartPointDto { Label = reader.GetString(0), Value = reader.GetDecimal(1) });
    return result;
}

public async Task<List<TopRecordDto>> GetTopRecordsAsync()
{
    const string sql = @"
        SELECT date, company, channel_name, revenue
        FROM revenues
        ORDER BY revenue DESC
        LIMIT 8;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<TopRecordDto>();
    while (await reader.ReadAsync())
        result.Add(new TopRecordDto
        {
            Date = reader.GetDateTime(0),
            Company = reader.GetString(1),
            ChannelName = reader.GetString(2),
            Revenue = reader.GetDecimal(3)
        });
    return result;
}
public async Task<List<ChartPointDto>> GetBottomDaysAsync()
{
    const string sql = @"
        SELECT TO_CHAR(date, 'YYYY-MM-DD') AS label,
               COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        GROUP BY date
        ORDER BY total ASC
        LIMIT 5;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
        result.Add(new ChartPointDto { Label = reader.GetString(0), Value = reader.GetDecimal(1) });
    return result;
}
public async Task<List<GrowthDto>> GetFastestGrowingChannelsAsync()
{
    const string sql = @"
        WITH ref AS (SELECT COALESCE(MAX(date), CURRENT_DATE) AS ref_date FROM revenues),
        this_month AS (
            SELECT channel_name, SUM(revenue) AS rev
            FROM revenues
            WHERE date >= date_trunc('month', (SELECT ref_date FROM ref))
            GROUP BY channel_name
        ),
        last_month AS (
            SELECT channel_name, SUM(revenue) AS rev
            FROM revenues
            WHERE date >= date_trunc('month', (SELECT ref_date FROM ref)) - INTERVAL '1 month'
              AND date < date_trunc('month', (SELECT ref_date FROM ref))
            GROUP BY channel_name
        )
        SELECT t.channel_name,
               t.rev AS current_rev,
               l.rev AS previous_rev,
               ((t.rev - l.rev) / l.rev * 100)::numeric AS growth
        FROM this_month t
        JOIN last_month l ON t.channel_name = l.channel_name
        WHERE l.rev >= 50000
        ORDER BY growth DESC
        LIMIT 5;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<GrowthDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new GrowthDto
        {
            Name = reader.GetString(0),
            CurrentRevenue = reader.GetDecimal(1),
            PreviousRevenue = reader.GetDecimal(2),
            GrowthPercent = reader.GetDecimal(3)
        });
    }

    return result;
}
public async Task<List<CompanyTrendDto>> GetCompanyMonthlyTrendsAsync()
{
    const string sql = @"
        SELECT company,
               TO_CHAR(date, 'YYYY-MM') AS month,
               COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE date >= date_trunc('month', CURRENT_DATE) - INTERVAL '11 months'
        GROUP BY company, TO_CHAR(date, 'YYYY-MM')
        ORDER BY company, month;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    // Şirket → (ay → gelir) topla
    var map = new Dictionary<string, CompanyTrendDto>();
    while (await reader.ReadAsync())
    {
        var company = reader.GetString(0);
        var month = reader.GetString(1);
        var total = reader.GetDecimal(2);

        if (!map.TryGetValue(company, out var dto))
        {
            dto = new CompanyTrendDto { Company = company };
            map[company] = dto;
        }
        dto.Months.Add(month);
        dto.Values.Add(total);
    }

    return map.Values.ToList();
}
public async Task<List<ParetoPointDto>> GetChannelParetoAsync()
{
    // Tüm kanalların toplam gelirini büyükten küçüğe çek
    const string sql = @"
        SELECT channel_name, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        GROUP BY channel_name
        ORDER BY total DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var all = new List<(string Label, decimal Value)>();
    while (await reader.ReadAsync())
        all.Add((reader.GetString(0), reader.GetDecimal(1)));

    var grandTotal = all.Sum(x => x.Value);
    var result = new List<ParetoPointDto>();
    decimal running = 0;

    // İlk 20 kanalı tek tek, geri kalanı "Diğer" olarak topla
    const int topN = 20;
    for (int i = 0; i < all.Count && i < topN; i++)
    {
        running += all[i].Value;
        result.Add(new ParetoPointDto
        {
            Label = all[i].Label,
            Value = all[i].Value,
            CumulativePercent = grandTotal > 0 ? running / grandTotal * 100 : 0
        });
    }

    if (all.Count > topN)
    {
        var otherSum = all.Skip(topN).Sum(x => x.Value);
        running += otherSum;
        result.Add(new ParetoPointDto
        {
            Label = "Diğer",
            Value = otherSum,
            CumulativePercent = grandTotal > 0 ? running / grandTotal * 100 : 0
        });
    }

    return result;
}
public async Task<List<CompanyEfficiencyDto>> GetCompanyEfficiencyAsync()
{
    const string sql = @"
        SELECT company,
               COALESCE(SUM(revenue), 0)::numeric AS total,
               COUNT(DISTINCT channel_name) AS channels
        FROM revenues
        GROUP BY company
        ORDER BY total DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<CompanyEfficiencyDto>();
    while (await reader.ReadAsync())
    {
        var total = reader.GetDecimal(1);
        var channels = reader.GetInt32(2);
        result.Add(new CompanyEfficiencyDto
        {
            Company = reader.GetString(0),
            TotalRevenue = total,
            ChannelCount = channels,
            RevenuePerChannel = channels > 0 ? total / channels : 0
        });
    }

    return result;
}
public async Task<List<CountryRevenueDto>> GetTopCountriesAsync()
{
    const string sql = @"
        SELECT country, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE country IS NOT NULL
        GROUP BY country
        ORDER BY total DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var all = new List<(string Country, decimal Total)>();
    while (await reader.ReadAsync())
        all.Add((reader.GetString(0), reader.GetDecimal(1)));

    // Her ülkenin ait olduğu kıtanın toplam gelirini hesapla (yüzde artık kıta içi pay)
    var continentTotals = all
        .GroupBy(c => RevenueDashboard.Infrastructure.ContinentData.CountryToContinent
            .TryGetValue(c.Country, out var cont) ? cont : "Diğer")
        .ToDictionary(g => g.Key, g => g.Sum(x => x.Total));

    var result = new List<CountryRevenueDto>();

    foreach (var c in all.Take(5))
    {
       var geo = RevenueDashboard.Infrastructure.CountryGeoData.Map.TryGetValue(c.Country, out var g)
    ? g
    : (Code: "un", Lat: 0.0, Lon: 0.0);

        var continent = RevenueDashboard.Infrastructure.ContinentData.CountryToContinent
            .TryGetValue(c.Country, out var cont2) ? cont2 : "Diğer";
        var continentTotal = continentTotals.TryGetValue(continent, out var ct) ? ct : 0m;

        result.Add(new CountryRevenueDto
        {
            Country = c.Country,
            CountryCode = geo.Code,
            Revenue = c.Total,
            Percentage = continentTotal > 0 ? (c.Total / continentTotal * 100) : 0,
            Latitude = geo.Lat,
            Longitude = geo.Lon
        });
    }

    return result;
}
public async Task<List<PlatformRevenueDto>> GetPlatformRevenueAsync()
{
    const string sql = @"
        SELECT platform, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE platform IS NOT NULL
        GROUP BY platform
        ORDER BY total DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var raw = new List<(string Platform, decimal Total)>();
    while (await reader.ReadAsync())
    {
        raw.Add((reader.GetString(0), reader.GetDecimal(1)));
    }

    var grandTotal = raw.Sum(x => x.Total);

    return raw.Select(x => new PlatformRevenueDto
    {
        Platform = x.Platform,
        Revenue = x.Total,
        Percentage = grandTotal > 0 ? (x.Total / grandTotal * 100) : 0
    }).ToList();
}

public async Task<List<CompanyPortfolioDto>> GetCompanyPortfolioAsync()
{
    const string sql = @"
        WITH channel_totals AS (
            SELECT company, channel_name, SUM(revenue) AS channel_revenue
            FROM revenues
            GROUP BY company, channel_name
        ),
        ranked AS (
            SELECT company, channel_name, channel_revenue,
                   ROW_NUMBER() OVER (PARTITION BY company ORDER BY channel_revenue DESC) AS rnk
            FROM channel_totals
        ),
        company_totals AS (
            SELECT company,
                   COUNT(DISTINCT channel_name)::int AS channel_count,
                   SUM(revenue) AS total_revenue
            FROM revenues
            GROUP BY company
        )
        SELECT ct.company, ct.channel_count, ct.total_revenue,
               r.channel_name AS top_content, r.channel_revenue AS top_content_revenue
        FROM company_totals ct
        JOIN ranked r ON r.company = ct.company AND r.rnk = 1
        ORDER BY ct.total_revenue DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<CompanyPortfolioDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new CompanyPortfolioDto
        {
            Company = reader.GetString(0),
            ChannelCount = reader.GetInt32(1),
            TotalRevenue = reader.GetDecimal(2),
            TopContent = reader.GetString(3),
            TopContentRevenue = reader.GetDecimal(4)
        });
    }

    return result;
}
public async Task<List<ContinentRevenueDto>> GetContinentRevenueAsync()
{
    const string sql = @"
        SELECT country, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues
        WHERE country IS NOT NULL
        GROUP BY country;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var countryTotals = new List<(string Country, decimal Total)>();
    while (await reader.ReadAsync())
        countryTotals.Add((reader.GetString(0), reader.GetDecimal(1)));

    var grouped = countryTotals
        .GroupBy(c => RevenueDashboard.Infrastructure.ContinentData.CountryToContinent
            .TryGetValue(c.Country, out var cont) ? cont : "Diğer")
        .Select(g => new { Continent = g.Key, Total = g.Sum(x => x.Total) })
        .OrderByDescending(x => x.Total)
        .ToList();

    var grandTotal = grouped.Sum(x => x.Total);

    return grouped.Select(g =>
    {
        var center = RevenueDashboard.Infrastructure.ContinentData.ContinentCenters
            .TryGetValue(g.Continent, out var c) ? c : (0.0, 0.0);

        return new ContinentRevenueDto
        {
            Continent = g.Continent,
            Revenue = g.Total,
            Percentage = grandTotal > 0 ? (g.Total / grandTotal * 100) : 0,
            Latitude = center.Item1,
            Longitude = center.Item2
        };
    }).ToList();
}
public async Task<List<ChartPointDto>> GetHeatmapRevenueAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS today FROM revenues)
        SELECT TO_CHAR(date, 'YYYY-MM-DD') AS day, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues, anchor
        WHERE date > anchor.today - INTERVAL '59 days' AND date <= anchor.today
        GROUP BY day
        ORDER BY day;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChartPointDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new ChartPointDto { Label = reader.GetString(0), Value = reader.GetDecimal(1) });
    }
    return result;
}

public async Task<List<CompanyPulseDto>> GetCompanyPulseAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS today FROM revenues)
        SELECT company, TO_CHAR(date, 'YYYY-MM-DD') AS day, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues, anchor
        WHERE date > anchor.today - INTERVAL '6 days' AND date <= anchor.today
        GROUP BY company, day
        ORDER BY company, day;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var rows = new List<(string Company, string Day, decimal Total)>();
    while (await reader.ReadAsync())
        rows.Add((reader.GetString(0), reader.GetString(1), reader.GetDecimal(2)));

    return rows
        .GroupBy(r => r.Company)
        .Select(g =>
        {
            var ordered = g.OrderBy(x => x.Day).Select(x => x.Total).ToList();
            var t = ordered.Count > 0 ? ordered[^1] : 0;
            var y = ordered.Count > 1 ? ordered[^2] : 0;

            return new CompanyPulseDto
            {
                Company = g.Key,
                TodayRevenue = t,
                YesterdayRevenue = y,
                ChangePercent = y > 0 ? (t - y) / y * 100 : 0,
                Sparkline = ordered
            };
        })
        .OrderByDescending(x => x.TodayRevenue)
        .ToList();
}

public async Task<List<MomentumDto>> GetMomentumAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS today FROM revenues),
        curr AS (
            SELECT channel_name, COALESCE(SUM(revenue), 0) AS total
            FROM revenues, anchor
            WHERE date > anchor.today - INTERVAL '7 days' AND date <= anchor.today
            GROUP BY channel_name
        ),
        prev AS (
            SELECT channel_name, COALESCE(SUM(revenue), 0) AS total
            FROM revenues, anchor
            WHERE date > anchor.today - INTERVAL '14 days' AND date <= anchor.today - INTERVAL '7 days'
            GROUP BY channel_name
        )
        SELECT c.channel_name, c.total AS curr_total, COALESCE(p.total, 0) AS prev_total
        FROM curr c
        LEFT JOIN prev p ON p.channel_name = c.channel_name;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<MomentumDto>();
    while (await reader.ReadAsync())
    {
        var curr = reader.GetDecimal(1);
        var prev = reader.GetDecimal(2);
        result.Add(new MomentumDto
        {
            Channel = reader.GetString(0),
            CurrentRevenue = curr,
            PreviousRevenue = prev,
            ChangePercent = prev > 0 ? (curr - prev) / prev * 100 : 0
        });
    }

    return result.Where(x => x.PreviousRevenue > 0).ToList();
}

public async Task<TodayGaugeDto> GetTodayGaugeAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS today FROM revenues)
        SELECT TO_CHAR(date, 'YYYY-MM-DD') AS day, COALESCE(SUM(revenue), 0)::numeric AS total
        FROM revenues, anchor
        WHERE date > anchor.today - INTERVAL '29 days' AND date <= anchor.today
        GROUP BY day
        ORDER BY day;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var days = new List<decimal>();
    while (await reader.ReadAsync())
        days.Add(reader.GetDecimal(1));

    if (days.Count == 0) return new TodayGaugeDto();

    var avg = days.Average();
    var today = days[^1];

    var streak = 0;
    for (int i = days.Count - 1; i >= 0; i--)
    {
        if (days[i] > avg) streak++;
        else break;
    }

    return new TodayGaugeDto
    {
        TodayRevenue = today,
        Average30 = avg,
        PercentOfAverage = avg > 0 ? (today / avg * 100) : 0,
        StreakDays = streak
    };
}
public async Task<List<CompanyMonthlyDto>> GetCompanyMonthlyAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS ref_date FROM revenues),
        totals AS (
            SELECT
                company,
                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date)
                      AND date <= anchor.ref_date
                ), 0)::numeric AS this_month,
                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month'
                      AND date <= (DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month')
                                   + (anchor.ref_date - DATE_TRUNC('month', anchor.ref_date))
                ), 0)::numeric AS last_month
            FROM revenues, anchor
            GROUP BY company
        ),
        monthly_series AS (
            SELECT company, TO_CHAR(date, 'YYYY-MM') AS ym, SUM(revenue)::numeric AS month_total
            FROM revenues, anchor
            WHERE date >= DATE_TRUNC('month', anchor.ref_date) - INTERVAL '6 months'
              AND date < DATE_TRUNC('month', anchor.ref_date)
            GROUP BY company, ym
        ),
        monthly_arrays AS (
            SELECT company, ARRAY_AGG(month_total ORDER BY ym) AS sparkline
            FROM monthly_series
            GROUP BY company
        )
        SELECT
            t.company,
            t.this_month,
            t.last_month,
            CASE WHEN t.last_month > 0 THEN ((t.this_month - t.last_month) / t.last_month * 100) ELSE 0 END::numeric AS change_percent,
            ma.sparkline
        FROM totals t
        LEFT JOIN monthly_arrays ma ON ma.company = t.company
        ORDER BY t.this_month DESC;
    ";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<CompanyMonthlyDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new CompanyMonthlyDto
        {
            Company = reader.GetString(0),
            ThisMonthRevenue = reader.GetDecimal(1),
            LastMonthRevenue = reader.GetDecimal(2),
            ChangePercent = reader.GetDecimal(3),
            Sparkline6Month = reader.IsDBNull(4)
                ? new List<decimal>()
                : reader.GetFieldValue<decimal[]>(4).ToList()
        });
    }

    return result;
}
public async Task<List<PlatformMonthlyDto>> GetPlatformMonthlyAsync()
{
    const string sql = @"
        WITH anchor AS (
            SELECT MAX(date) AS ref_date
            FROM revenues
        ),

        platform_totals AS (
            SELECT
                platform,

                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date)
                      AND date <= anchor.ref_date
                ), 0)::numeric AS this_month,

                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month'
                      AND date <= (DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month')
                                   + (anchor.ref_date - DATE_TRUNC('month', anchor.ref_date))
                ), 0)::numeric AS last_month

            FROM revenues, anchor

            WHERE platform IN ('YouTube', 'Facebook')

            GROUP BY platform
        ),

        channel_totals AS (
            SELECT
                platform,
                channel_name,
                SUM(revenue)::numeric AS channel_revenue,

                ROW_NUMBER() OVER (
                    PARTITION BY platform
                    ORDER BY SUM(revenue) DESC
                ) AS rn

            FROM revenues, anchor

            WHERE platform IN ('YouTube', 'Facebook')
              AND date >= DATE_TRUNC('month', anchor.ref_date)
              AND date <= anchor.ref_date

            GROUP BY platform, channel_name
        ),

        monthly_series AS (
            SELECT
                platform,
                TO_CHAR(date, 'YYYY-MM') AS ym,
                SUM(revenue)::numeric AS month_total
            FROM revenues, anchor
            WHERE platform IN ('YouTube', 'Facebook')
              AND date >= DATE_TRUNC('month', anchor.ref_date) - INTERVAL '12 months'
              AND date < DATE_TRUNC('month', anchor.ref_date)
            GROUP BY platform, ym
        ),

        monthly_arrays AS (
            SELECT
                platform,
                ARRAY_AGG(month_total ORDER BY ym) AS sparkline
            FROM monthly_series
            GROUP BY platform
        )

        SELECT
            p.platform,
            p.this_month,
            p.last_month,

            CASE
                WHEN p.last_month > 0
                THEN ((p.this_month - p.last_month) / p.last_month * 100)
                ELSE 0
            END::numeric AS change_percent,

            COALESCE(c.channel_name, '-') AS top_channel,
            COALESCE(c.channel_revenue, 0)::numeric AS top_channel_revenue,
            ma.sparkline AS sparkline

        FROM platform_totals p

        LEFT JOIN channel_totals c
            ON c.platform = p.platform
           AND c.rn = 1

        LEFT JOIN monthly_arrays ma
            ON ma.platform = p.platform

        ORDER BY p.this_month DESC;
    ";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<PlatformMonthlyDto>();

    while (await reader.ReadAsync())
    {
        result.Add(new PlatformMonthlyDto
        {
            Platform = reader.GetString(0),
            ThisMonthRevenue = reader.GetDecimal(1),
            LastMonthRevenue = reader.GetDecimal(2),
            ChangePercent = reader.GetDecimal(3),
            TopChannel = reader.GetString(4),
            TopChannelRevenue = reader.GetDecimal(5),
            Sparkline12Month = reader.IsDBNull(6)
                ? new List<decimal>()
                : reader.GetFieldValue<decimal[]>(6).ToList()
        });
    }

    return result;
}
public async Task<List<ChannelMonthlyDto>> GetTopChannelsMonthlyAsync()
{
    const string sql = @"
        WITH anchor AS (
            SELECT MAX(date) AS ref_date
            FROM revenues
        ),
        channel_data AS (
            SELECT
                channel_name,
                company,

                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date)
                      AND date <= anchor.ref_date
                ), 0)::numeric AS this_month,

                COALESCE(SUM(revenue) FILTER (
                    WHERE date >= DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month'
                      AND date <= (DATE_TRUNC('month', anchor.ref_date) - INTERVAL '1 month')
                                   + (anchor.ref_date - DATE_TRUNC('month', anchor.ref_date))
                ), 0)::numeric AS last_month

            FROM revenues, anchor
            GROUP BY channel_name, company
        )

        SELECT
            channel_name,
            company,
            this_month,
            last_month,

            CASE
                WHEN last_month > 0
                THEN ((this_month - last_month) / last_month * 100)
                ELSE 0
            END::numeric AS change_percent

        FROM channel_data

        WHERE this_month > 0

        ORDER BY this_month DESC

        LIMIT 5;
    ";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<ChannelMonthlyDto>();

    while (await reader.ReadAsync())
    {
        result.Add(new ChannelMonthlyDto
        {
            ChannelName = reader.GetString(0),
            Company = reader.GetString(1),
            ThisMonthRevenue = reader.GetDecimal(2),
            LastMonthRevenue = reader.GetDecimal(3),
            ChangePercent = reader.GetDecimal(4)
        });
    }

    return result;
}
public async Task<List<ContinentSeriesDto>> GetContinentTrendAsync()
{
    const string sql = @"
        WITH anchor AS (SELECT MAX(date) AS today FROM revenues),
        top5 AS (
            SELECT country, SUM(revenue) AS total
            FROM revenues, anchor
            WHERE country IS NOT NULL
              AND date > anchor.today - INTERVAL '29 days' AND date <= anchor.today
            GROUP BY country
            ORDER BY total DESC
            LIMIT 5
        )
        SELECT r.country, TO_CHAR(r.date, 'YYYY-MM-DD') AS day, COALESCE(SUM(r.revenue), 0)::numeric AS total
        FROM revenues r
        CROSS JOIN anchor
        JOIN top5 t ON t.country = r.country
        WHERE r.date > anchor.today - INTERVAL '29 days' AND r.date <= anchor.today
        GROUP BY r.country, day
        ORDER BY r.country, day;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var rows = new List<(string Country, string Day, decimal Total)>();
    while (await reader.ReadAsync())
        rows.Add((reader.GetString(0), reader.GetString(1), reader.GetDecimal(2)));

    return rows
        .GroupBy(r => r.Country)
        .Select(g => new ContinentSeriesDto
        {
            Continent = g.Key,
            Points = g.Select(x => new ChartPointDto { Label = x.Day, Value = x.Total }).OrderBy(p => p.Label).ToList()
        })
        .ToList();
}

public async Task<List<GrowthDto>> GetTopMoversAsync()
{
    const string sql = @"
        WITH ref AS (SELECT COALESCE(MAX(date), CURRENT_DATE) AS ref_date FROM revenues),
        this_month AS (
            SELECT channel_name, SUM(revenue) AS rev
            FROM revenues
            WHERE date >= date_trunc('month', (SELECT ref_date FROM ref))
            GROUP BY channel_name
        ),
        last_month AS (
            SELECT channel_name, SUM(revenue) AS rev
            FROM revenues
            WHERE date >= date_trunc('month', (SELECT ref_date FROM ref)) - INTERVAL '1 month'
              AND date < date_trunc('month', (SELECT ref_date FROM ref))
            GROUP BY channel_name
        ),
        changes AS (
            SELECT t.channel_name,
                   t.rev AS current_rev,
                   l.rev AS previous_rev,
                   ((t.rev - l.rev) / l.rev * 100)::numeric AS growth
            FROM this_month t
            JOIN last_month l ON t.channel_name = l.channel_name
            WHERE l.rev >= 20000
        ),
        gainers AS (
            SELECT * FROM changes WHERE growth >= 0 ORDER BY growth DESC LIMIT 10
        ),
        losers AS (
            SELECT * FROM changes WHERE growth < 0 ORDER BY growth ASC LIMIT 10
        )
        SELECT channel_name, current_rev, previous_rev, growth FROM gainers
        UNION ALL
        SELECT channel_name, current_rev, previous_rev, growth FROM losers
        ORDER BY growth DESC;";

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();

    var result = new List<GrowthDto>();
    while (await reader.ReadAsync())
    {
        result.Add(new GrowthDto
        {
            Name = reader.GetString(0),
            CurrentRevenue = reader.GetDecimal(1),
            PreviousRevenue = reader.GetDecimal(2),
            GrowthPercent = reader.GetDecimal(3)
        });
    }

    return result;
}

}
