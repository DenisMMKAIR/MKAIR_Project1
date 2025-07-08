using System.Text.Json.Serialization;

namespace ProjApp.Database.EntitiesStatic;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProtocolGroup
{
    Манометры,
    Датчики_давления,
    Термометры_биметаллические
}