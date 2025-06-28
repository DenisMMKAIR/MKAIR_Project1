using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Infrastructure.FGISAPI.Converters;

namespace Infrastructure.FGISAPI;

internal static class AppConfig
{
    //public static string APIUrl { get; } = "https://fgis.gost.ru/fundmetrologytest/eapi";
    public static string APIUrl { get; } = "https://fgis.gost.ru/fundmetrology/eapi";

    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DateOnlyConverter() },
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
}