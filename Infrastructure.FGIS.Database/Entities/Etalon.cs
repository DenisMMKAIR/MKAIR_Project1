namespace Infrastructure.FGIS.Database.Entities;

public record Etalon(
    string Number,
    string MiType_Num,
    string MiType,
    string MiNotation,
    string Modification,
    string Factory_Num,
    int Year,
    string Schematype,
    string Schematitle,
    string NpEnumber,
    string RankCode,
    string RankClass,
    bool Applicability
)
{
    public required IReadOnlyList<EtalonVerificationDocs> CResults { get; set; }

    public record EtalonVerificationDocs(
        string Vri_Id,
        string Org_Title,
        string Verification_Date,
        string Valid_Date,
        string Result_Docnum,
        bool Applicability
    );
}