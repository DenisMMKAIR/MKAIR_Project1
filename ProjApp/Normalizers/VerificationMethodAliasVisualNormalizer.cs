using System.Text.RegularExpressions;

namespace ProjApp.Normalizers;

public partial class VerificationMethodAliasVisualNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"[^А-Яа-яA-Za-z0-9\-\. ]", RegexOptions.IgnoreCase)]
    private static partial Regex NameBannedSymbols();

    public string Normalize(string value)
    {
        value = DotsAndComasNormalizer.Instance.Normalize(value);
        value = SpaceNormalizer.Instance.Normalize(value);
        value = NameBannedSymbols().Replace(value, "");
        return value.ToUpper();
    }

    public static IStringNormalizer Instance { get; } = new VerificationMethodAliasVisualNormalizer();
}
