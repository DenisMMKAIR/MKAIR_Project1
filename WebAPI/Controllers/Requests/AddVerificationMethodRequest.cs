using ProjApp.Database.EntitiesStatic;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, Dictionary<VerificationMethodCheckups, string> Checkups);

public record VerificationMethodFileRequest(string? FileName, IFormFile? File);