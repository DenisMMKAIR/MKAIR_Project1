using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Name, string Description, IReadOnlyList<string> Aliases, IReadOnlyList<string> Checkups)
{
    public VerificationMethod MapToVerificationMethod()
    {
        return new()
        {
            Name = Name,
            Description = Description,
            Aliases = [new() { Name = Name }],
            Checkups = Checkups
        };
    }
}
