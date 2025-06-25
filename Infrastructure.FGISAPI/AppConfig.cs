using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Infrastructure.FGISAPI;

internal static class AppConfig
{
    public static string APIUrl { get; } = "https://fgis.gost.ru/fundmetrologytest/eapi";
    //public static string APIUrl { get; } = "https://fgis.gost.ru/fundmetrology/eapi";

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
}