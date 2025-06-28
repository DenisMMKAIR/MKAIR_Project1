using ProjApp.Database.SupportTypes;

namespace ProjApp.Database.Entities;

public class InitialVerificationJob : DatabaseEntity
{
    public required YearMonth Date { get; set; }
}
