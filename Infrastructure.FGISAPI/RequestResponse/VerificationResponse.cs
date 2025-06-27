using Infrastructure.FGIS.Database.Entities;

namespace Infrastructure.FGISAPI.RequestResponse;

public class VerificationResponse
{
    public required Verification Result { get; set; }
}
