using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class WorkerNameNormalizer : IColumnNormalizer
{
    [GeneratedRegex(@"^([А-Яа-я]+)\s+([А-Я]\.)\s*([А-Я]\.)\s*$")]
    private static partial Regex NameRegex();

    public string Normalize(string value)
    {
        var m = NameRegex().Match(value);
        return $"{m.Groups[1].Value} {m.Groups[2].Value}{m.Groups[3].Value}";
    }
}
