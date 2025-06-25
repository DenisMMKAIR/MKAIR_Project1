using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsNormalizers;

internal partial class SingleFloatDigitColumnNormalizer : IColumnNormalizer
{
    public string Normalize(string value)
    {
        return MyRegex().Match(value).Value.Replace('.', ',');
    }

    [GeneratedRegex(@"-?[ ]*\d+[\.,]?\d*")]
    private static partial Regex MyRegex();
}
