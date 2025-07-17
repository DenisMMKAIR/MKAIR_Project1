using ProjApp.Database.SupportTypes;

namespace Infrastructure.FGIS.Database.Entities;

public record MonthResult(YearMonth Date)
{
    public bool VerificationIdsCollected { get; set; }
    public bool VerificationsCollected { get; set; }
    public bool EtalonsIdsCollected { get; set; }
    public bool EtalonsCollected { get; set; }
    public bool DeviceTypeIdsCollected { get; set; }
    public bool DeviceTypesCollected { get; set; }
}
