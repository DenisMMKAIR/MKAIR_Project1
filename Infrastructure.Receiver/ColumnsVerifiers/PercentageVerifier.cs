using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class PercentageVerifier : IColumnVerifier
{
    public string ErrorMessage { get; } = "Ошибка ввода процента";

    public bool Verify(string value)
    {
        if (PercentageRegex().IsMatch(value)) return true;

        if (!float.TryParse(value, out var result)) return false;
        return result >= 0 && result <= 1;
    }

    [GeneratedRegex(@"^[0-9]+[\.,0-9]+?\s*%$")] private static partial Regex PercentageRegex();
}
