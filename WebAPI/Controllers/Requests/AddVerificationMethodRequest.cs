using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, IReadOnlyList<string> Checkups)
{
    public VerificationMethod MapToVerificationMethod()
    {
        return new()
        {
            Description = Description,
            Aliases = [.. Aliases.Select(a => new VerificationMethodAlias { Name = a })],
            Checkups = Checkups
        };
    }
}
