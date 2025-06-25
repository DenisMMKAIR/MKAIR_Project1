using System.Text.Json.Serialization;

namespace ProjApp.Database;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeviceLocation
{
    АнтипинскийНПЗ,
    ГПНЯмал
}
