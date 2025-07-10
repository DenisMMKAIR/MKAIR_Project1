using System.Text.RegularExpressions;

namespace Infrastructure.Receiver.ColumnsVerifiers;

public partial class WorkerNameVerifier : IColumnVerifier
{
    public string ErrorMessage => "Имя имеет недопустимый формат";
    [GeneratedRegex(@"^[А-Яа-я]+ [А-Яа-я]\.[А-Яа-я]\.$")]
    private static partial Regex NameRegex();

    public bool Verify(string value)
    {
        return NameRegex().IsMatch(value);
    }
}
