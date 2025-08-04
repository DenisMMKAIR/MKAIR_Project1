using System.Text.RegularExpressions;

namespace ProjApp.Normalizers.VerificationMethod;

public partial class VerificationMethodCheckupNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"[^А-Яа-яA-Za-z0-9\-\. ]", RegexOptions.IgnoreCase)]
    private static partial Regex bannedSymbols();

    public string Normalize(string value)
    {
        value = SpaceNormalizer.Instance.Normalize(value);
        value = bannedSymbols().Replace(value, "");
        value = string.Concat(value[0].ToString().ToUpper(), value.AsSpan(1));
        return value;
    }

    public static IStringNormalizer Instance { get; } = new VerificationMethodCheckupNormalizer();
}
