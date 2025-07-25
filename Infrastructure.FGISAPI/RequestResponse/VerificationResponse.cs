namespace Infrastructure.FGISAPI.RequestResponse;

public class VerificationResponse
{
    public required VerificationResult Result { get; set; }
}

public class VerificationResult
{
    public required MiInfoClass MiInfo { get; set; }
    public required VriInfoClass VriInfo { get; set; }
    public required MeansClass Means { get; set; }
    public required InfoClass Info { get; set; }

    public class MiInfoClass
    {
        public required SingleMI SingleMI { get; set; }
    }

    public class VriInfoClass
    {
        public required string Organization { get; set; }
        public required string SignCipher { get; set; }
        public required string MiOwner { get; set; }
        public required DateOnly VrfDate { get; set; }
        public required string VriType { get; set; }
        public required string DocTitle { get; set; }
        public Applicable? Applicable { get; set; }
        public Inapplicable? Inapplicable { get; set; }
        public DateOnly? ValidDate { get; set; }
    }

    public class MeansClass
    {
        public IReadOnlyList<Mietum>? Mieta { get; set; }
        public IReadOnlyList<Sample>? Ses { get; set; }
    }

    public class InfoClass
    {
        public required bool BriefIndicator { get; set; }
        public string? Additional_Info { get; set; }
    }

    public class Applicable
    {
        public required string CertNum { get; set; }
        public required bool SignPass { get; set; }
        public required bool SignMi { get; set; }
    }

    public class Inapplicable
    {
        public required string NoticeNum { get; set; }
    }

    public class Mietum
    {
        public required string RegNumber { get; set; }
        public required string MietaURL { get; set; }
        public required string MitypeNumber { get; set; }
        public required string MitypeURL { get; set; }
        public required string MitypeTitle { get; set; }
        public required string Notation { get; set; }
        public required string Modification { get; set; }
        public required string ManufactureNum { get; set; }
        public required int ManufactureYear { get; set; }
        public required string RankCode { get; set; }
        public required string RankTitle { get; set; }
        public required string SchemaTitle { get; set; }
    }
    
    public class Sample
    {
        public required string Number { get; set; }
        public required string Title { get; set; }
        public required string SeURL { get; set; }
        public required int ManufactureYear { get; set; }
    }

    public class SingleMI
    {
        public required string MitypeNumber { get; set; }
        public required string MitypeURL { get; set; }
        public required string MitypeType { get; set; }
        public required string MitypeTitle { get; set; }
        public required string ManufactureNum { get; set; }
        public required int ManufactureYear { get; set; }
        public required string Modification { get; set; }
    }
}
