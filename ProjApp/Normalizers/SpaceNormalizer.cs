using System.Text.RegularExpressions;

namespace ProjApp.Normalizers;

public partial class SpaceNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"\s+")] private static partial Regex _aliasNameSpaces();
    public string Normalize(string value) => _aliasNameSpaces().Replace(value, " ");
}
