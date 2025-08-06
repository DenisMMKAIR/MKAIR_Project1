using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class WorkerNameVerifier : IColumnVerifier
{
    public string ErrorMessage => "Имя имеет недопустимый формат";
    [GeneratedRegex(@"^[а-я]+\s+[а-я]\.?\s*[а-я]\.?\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex NameRegex();

    public bool Verify(string value)
    {
        return NameRegex().IsMatch(value);
    }
}
