using System.Text.Json.Serialization;

namespace ProjApp.Database.EntitiesStatic;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShowVMethods
{
    Новые,
    Частичные,
    Все
}