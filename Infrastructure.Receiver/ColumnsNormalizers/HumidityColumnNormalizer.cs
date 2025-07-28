using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsNormalizers;

internal partial class HumidityColumnNormalizer : IColumnNormalizer
{
    public string Normalize(string value)
    {
        value = value.Replace('.', ',').Trim();

        var textPercantage = TextPercentageRegex().Match(value);
        if (textPercantage.Success)
        {
            return (float.Parse(textPercantage.Groups[1].Value) / 100).ToString();
        }
        
        return value;
    }

    [GeneratedRegex(@"^([0-9]+[,0-9]+?)\s*%$")] private static partial Regex TextPercentageRegex();
}
