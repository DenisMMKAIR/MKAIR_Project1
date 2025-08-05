using ProjApp.Database.Entities;

namespace WebAPI.Controllers.Requests;

public record AddVerificationMethodRequest(string Description, IReadOnlyList<string> Aliases, Dictionary<string, CheckupType> Checkups);

public record VerificationMethodFileRequest(string? FileName, IFormFile? File);