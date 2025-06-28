using Infrastructure.FGIS.Database.Entities;

namespace Infrastructure.FGISAPI.RequestResponse;

public static class VerificationMapper
{
    public static Verification ToVerification(this VerificationResult verification)
    {
        return new Verification
        {
            Vri_id = verification.Vri_id,
            MiInfo = new Verification.MiInfoClass
            {
                SingleMI = new Verification.SingleMI
                {
                    MitypeNumber = verification.MiInfo.SingleMI.MitypeNumber,
                    MitypeURL = verification.MiInfo.SingleMI.MitypeURL,
                    MitypeType = verification.MiInfo.SingleMI.MitypeType,
                    MitypeTitle = verification.MiInfo.SingleMI.MitypeTitle,
                    ManufactureNum = verification.MiInfo.SingleMI.ManufactureNum,
                    ManufactureYear = verification.MiInfo.SingleMI.ManufactureYear,
                    Modification = verification.MiInfo.SingleMI.Modification
                }
            },
            VriInfo = new Verification.VriInfoClass
            {
                Organization = verification.VriInfo.Organization,
                SignCipher = verification.VriInfo.SignCipher,
                MiOwner = verification.VriInfo.MiOwner,
                VrfDate = verification.VriInfo.VrfDate,
                ValidDate = verification.VriInfo.ValidDate,
                VriType = verification.VriInfo.VriType,
                DocTitle = verification.VriInfo.DocTitle,
                Applicable = new Verification.Applicable
                {
                    CertNum = verification.VriInfo.Applicable.CertNum,
                    SignPass = verification.VriInfo.Applicable.SignPass,
                    SignMi = verification.VriInfo.Applicable.SignMi
                }
            },
            Means = new Verification.MeansClass
            {
                Mieta = verification.Means.Mieta
                    .Select(m => new Verification.Mietum
                    {
                        RegNumber = m.RegNumber,
                        MietaURL = m.MietaURL,
                        MitypeNumber = m.MitypeNumber,
                        MitypeURL = m.MitypeURL,
                        MitypeTitle = m.MitypeTitle,
                        Notation = m.Notation,
                        Modification = m.Modification,
                        ManufactureNum = m.ManufactureNum,
                        ManufactureYear = m.ManufactureYear,
                        RankCode = m.RankCode,
                        RankTitle = m.RankTitle,
                        SchemaTitle = m.SchemaTitle
                    }).ToList()
            },
            Info = new Verification.InfoClass
            {
                BriefIndicator = verification.Info.BriefIndicator,
                Additional_Info = verification.Info.Additional_Info
            }
        };
    }

    public static VerificationResult ToVerificationResult(this Verification verification)
    {
        return new VerificationResult
        {
            Vri_id = verification.Vri_id,
            MiInfo = new VerificationResult.MiInfoClass
            {
                SingleMI = new VerificationResult.SingleMI
                {
                    MitypeNumber = verification.MiInfo.SingleMI.MitypeNumber,
                    MitypeURL = verification.MiInfo.SingleMI.MitypeURL,
                    MitypeType = verification.MiInfo.SingleMI.MitypeType,
                    MitypeTitle = verification.MiInfo.SingleMI.MitypeTitle,
                    ManufactureNum = verification.MiInfo.SingleMI.ManufactureNum,
                    ManufactureYear = verification.MiInfo.SingleMI.ManufactureYear,
                    Modification = verification.MiInfo.SingleMI.Modification
                }
            },
            VriInfo = new VerificationResult.VriInfoClass
            {
                Organization = verification.VriInfo.Organization,
                SignCipher = verification.VriInfo.SignCipher,
                MiOwner = verification.VriInfo.MiOwner,
                VrfDate = verification.VriInfo.VrfDate,
                ValidDate = verification.VriInfo.ValidDate,
                VriType = verification.VriInfo.VriType,
                DocTitle = verification.VriInfo.DocTitle,
                Applicable = new VerificationResult.Applicable
                {
                    CertNum = verification.VriInfo.Applicable.CertNum,
                    SignPass = verification.VriInfo.Applicable.SignPass,
                    SignMi = verification.VriInfo.Applicable.SignMi
                }
            },
            Means = new VerificationResult.MeansClass
            {
                Mieta = verification.Means.Mieta
                    .Select(m => new VerificationResult.Mietum
                    {
                        RegNumber = m.RegNumber,
                        MietaURL = m.MietaURL,
                        MitypeNumber = m.MitypeNumber,
                        MitypeURL = m.MitypeURL,
                        MitypeTitle = m.MitypeTitle,
                        Notation = m.Notation,
                        Modification = m.Modification,
                        ManufactureNum = m.ManufactureNum,
                        ManufactureYear = m.ManufactureYear,
                        RankCode = m.RankCode,
                        RankTitle = m.RankTitle,
                        SchemaTitle = m.SchemaTitle
                    }).ToList()
            },
            Info = new VerificationResult.InfoClass
            {
                BriefIndicator = verification.Info.BriefIndicator,
                Additional_Info = verification.Info.Additional_Info
            }
        };
    }
}
