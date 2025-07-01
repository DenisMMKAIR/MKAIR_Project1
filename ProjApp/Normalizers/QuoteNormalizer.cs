using System.Text.RegularExpressions;

namespace ProjApp.Normalizers;

public partial class QuoteNormalizer : IStringNormalizer
{
    [GeneratedRegex(@"[«""»]")] private static partial Regex _aliasNameNoQuotes();
    public string Normalize(string value) => _aliasNameNoQuotes().Replace(value, "");
}
