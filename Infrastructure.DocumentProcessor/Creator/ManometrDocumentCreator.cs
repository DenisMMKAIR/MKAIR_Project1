using AngleSharp.Dom;
using Infrastructure.SharedCode;
using System.Reflection;

namespace Infrastructure.DocumentProcessor.Creator;

internal class ManometrDocumentCreator : DocumentCreatorBase<ManometrData>
{
    protected override IReadOnlyList<PropertyInfo> TypeProps { get; init; } = typeof(ManometrData).GetProperties();
    protected override int VerificationLineLength { get; init; } = 75;
    protected override int EtalonLineLength { get; init; } = 100;
    protected override int AdditionalLineLength { get; init; } = 130;

    public ManometrDocumentCreator(string signsDirPath) : base(signsDirPath, "Forms/Manometr.html") { }

    protected override Task<string?> SetSpecificAsync(IDocument document, ManometrData data)
    {
        return Task.FromResult<string?>(null);
    }
}
