using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class WorkerNameNormalizer : IColumnNormalizer
{
    [GeneratedRegex(@"^([а-я]+)\s+([а-я])\.?\s*([а-я])\.?\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex NameRegex();

    public string Normalize(string value)
    {
        var m = NameRegex().Match(value);
        var surname = $"{char.ToUpper(m.Groups[1].Value[0])}{m.Groups[1].Value[1..].ToLower()}";
        return $"{surname} {m.Groups[2].Value.ToUpper()}.{m.Groups[3].Value.ToUpper()}.";
    }
}
