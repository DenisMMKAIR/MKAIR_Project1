namespace ProjApp.Database.Entities;

public class Etalon : DatabaseEntity
{
    public required string Number { get; set; }
    public required DateOnly Date { get; set; }
    public required DateOnly ToDate { get; set; }
    public required string FullInfo { get; set; }

    // Navigation properties
    public ICollection<SuccessInitialVerification>? SuccessInitialVerifications { get; set; }
    public ICollection<FailedInitialVerification>? FailedInitialVerifications { get; set; }
    public ICollection<Manometr1Verification>? Manometr1Verifications { get; set; }
    public ICollection<Davlenie1Verification>? Davlenie1Verifications { get; set; }
}
