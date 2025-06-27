namespace Infrastructure.FGIS.Database.Entities;

public record MonthResult(DateOnly Date)
{
    public bool Done { get; set; }
    public bool VerificationIdsCollected { get; set; }
    public bool VerificationsCollected { get; set; }
    public bool EtalonsIdsCollected { get; set; }
    public bool EtalonsCollected { get; set; }
}
