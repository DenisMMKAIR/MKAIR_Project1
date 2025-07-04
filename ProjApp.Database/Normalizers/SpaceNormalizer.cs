using System.Text.RegularExpressions;

namespace ProjApp.Database.Normalizers;

public partial class SpaceNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"\s+")] private static partial Regex _aliasNameSpaces();
    public string Normalize(string value) => _aliasNameSpaces().Replace(value, " ");
}
