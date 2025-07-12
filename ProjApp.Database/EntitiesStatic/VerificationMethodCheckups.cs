using System.Text.Json.Serialization;

namespace ProjApp.Database.EntitiesStatic;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VerificationMethodCheckups
{
    внешний_осмотр,
    результат_опробывания,
    опр_осн_поргрешности
}