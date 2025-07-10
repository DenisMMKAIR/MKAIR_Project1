using AngleSharp.Dom;
using ProjApp.Database.Entities;
using System.Reflection;

namespace Infrastructure.DocumentProcessor.Creator;

internal class ManometrSuccessDocumentCreator : DocumentCreatorBase<Manometr1Verification>
{
    protected override IReadOnlyList<PropertyInfo> TypeProps { get; init; } = typeof(Manometr1Verification).GetProperties();
    protected override int VerificationLineLength { get; init; } = 75;
    protected override int EtalonLineLength { get; init; } = 100;
    protected override int AdditionalLineLength { get; init; } = 130;

    public ManometrSuccessDocumentCreator(Dictionary<string, string> signsCache, string signsDirPath) : base(signsCache, signsDirPath, "Forms/Manometr.html") { }

    protected override Task<string?> SetSpecificAsync(IDocument document, Manometr1Verification data)
    {
        return Task.FromResult<string?>(null);
    }
}
