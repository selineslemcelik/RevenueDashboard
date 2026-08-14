namespace RevenueDashboard.Infrastructure;

public static class CountryGeoData
{
    // Excel/DB'deki ülke adı -> (ISO kodu, enlem, boylam)
    public static readonly Dictionary<string, (string Code, double Lat, double Lon)> Map = new()
    {
        ["Türkiye"] = ("tr", 39.0000, 35.0000),
        ["Meksika"] = ("mx", 23.6345, -102.5528),
        ["İspanya"] = ("es", 40.4637, -3.7492),
        ["Suudi Arabistan"] = ("sa", 23.8859, 45.0792),
        ["Brezilya"] = ("br", -14.2350, -51.9253),
        ["ABD"] = ("us", 37.0902, -95.7129),
        ["Almanya"] = ("de", 51.1657, 10.4515),
        ["Şili"] = ("cl", -35.6751, -71.5430),
    };

    public const double HqLat = 41.0082; // İstanbul HQ
    public const double HqLon = 28.9784;
}