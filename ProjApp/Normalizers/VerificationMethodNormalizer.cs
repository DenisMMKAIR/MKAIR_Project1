using System.Text.RegularExpressions;

namespace ProjApp.Normalizers;

public partial class VerificationMethodAliasNormalizer : IStringNormalizer
{
    [GeneratedRegex("[^А-Яа-яA-Za-z0-9]", RegexOptions.IgnoreCase)]
    private static partial Regex VrfNameBannedSymbolsRegex();
    [GeneratedRegex("^МИ|^МП|МИ$|МП$", RegexOptions.IgnoreCase)]
    private static partial Regex VrfNameTrimRegex();

    public string Normalize(string value)
    {
        return VrfNameTrimRegex().Replace(VrfNameBannedSymbolsRegex().Replace(value, ""), "").ToUpper();
    }

    public static IStringNormalizer Instance { get; } = new VerificationMethodAliasNormalizer();
}
