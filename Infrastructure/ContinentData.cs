namespace RevenueDashboard.Infrastructure;

public static class ContinentData
{
    public static readonly Dictionary<string, string> CountryToContinent = new()
    {
        ["Türkiye"] = "Avrupa",
        ["Almanya"] = "Avrupa",
        ["İspanya"] = "Avrupa",
        ["ABD"] = "Kuzey Amerika",
        ["Meksika"] = "Kuzey Amerika",
        ["Brezilya"] = "Güney Amerika",
        ["Şili"] = "Güney Amerika",
        ["Suudi Arabistan"] = "Asya",
    };

    public static readonly Dictionary<string, (double Lat, double Lon)> ContinentCenters = new()
    {
        ["Avrupa"] = (50.0, 15.0),
        ["Kuzey Amerika"] = (40.0, -100.0),
        ["Güney Amerika"] = (-15.0, -60.0),
        ["Asya"] = (30.0, 60.0),
        ["Afrika"] = (5.0, 20.0),
        ["Okyanusya"] = (-25.0, 135.0),
    };
}