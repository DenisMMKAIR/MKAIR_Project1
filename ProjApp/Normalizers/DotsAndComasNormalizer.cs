using System.Text.RegularExpressions;

namespace ProjApp.Normalizers;

public partial class DotsAndComasNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"\.+")] private static partial Regex _aliasNameDots();
    [GeneratedRegex(@"\,+")] private static partial Regex _aliasNameComas();
    public string Normalize(string value)
    {
        return _aliasNameComas().Replace(_aliasNameDots().Replace(value, "."), ",");
    }

    public static IStringNormalizer Instance { get; } = new DotsAndComasNormalizer();
}