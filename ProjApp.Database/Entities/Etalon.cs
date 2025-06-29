namespace ProjApp.Database.Entities;

public class Etalon : DatabaseEntity
{
    public required string Number { get; set; }
    public required DateOnly Date { get; set; }
    public required DateOnly ToDate { get; set; }
    public required string FullInfo { get; set; }
    public IReadOnlyList<InitialVerification>? InitialVerifications { get; set; }
}
