using System.Text.Json.Serialization;

namespace ProjApp.Database.EntitiesStatic;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VerificationMethodCheckups
{
    внешний_осмотр,
    опробование,
    осн_поргрешность,
    осн_привед_погр_и_вариации,
    вариации_выходн_сигн,
    прогр_обеспеч,
    гермет_сист,
    электр_прочн_и_сопр_изоляции,
    проверка_данных_ПО
}

public static class VerificationMethodCheckupsExtensions
{
    public static string GetDescription(this VerificationMethodCheckups value)
    {
        return value switch
        {
            VerificationMethodCheckups.внешний_осмотр => "Результат внешнего осмотра",
            VerificationMethodCheckups.опробование => "Результат опробования",
            VerificationMethodCheckups.осн_поргрешность => "Определение основной погрешности",
            VerificationMethodCheckups.осн_привед_погр_и_вариации => "Определение основной приведенной погрешности и вариации",
            VerificationMethodCheckups.вариации_выходн_сигн => "Определение вариации выходного сигнала",
            VerificationMethodCheckups.прогр_обеспеч => "Проверка программного обеспечения",
            VerificationMethodCheckups.гермет_сист => "Проверка герметичности системы",
            VerificationMethodCheckups.электр_прочн_и_сопр_изоляции => "Проверка электрической прочности и сопротивления изоляции",
            VerificationMethodCheckups.проверка_данных_ПО => "Проверка идентификационных данных ПО",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
