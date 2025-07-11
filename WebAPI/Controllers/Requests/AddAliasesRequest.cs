namespace WebAPI.Controllers.Requests;

public record AddAliasesRequest(IReadOnlyList<string> Aliases, Guid VerificationMethodId);