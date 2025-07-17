using Infrastructure.FGIS.Database.Entities;

namespace Infrastructure.FGISAPI.RequestResponse;

public static class VerificationMapper
{
    public static VerificationWithEtalon ToEtaVerification(this VerificationResult verification, string vri_id)
    {
        var goodVrf = verification.VriInfo.Applicable != null;
        var failedVrf = verification.VriInfo.Inapplicable != null;

        if (goodVrf == failedVrf)
        {
            throw new Exception("Поверка должна быть либо хорошая, либо неудачная");
        }

        return new VerificationWithEtalon
        {
            Vri_id = vri_id,
            MiInfo = new VerificationWithEtalon.MiInfoClass
            {
                SingleMI = new VerificationWithEtalon.SingleMI
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
            VriInfo = new VerificationWithEtalon.VriInfoClass
            {
                Organization = verification.VriInfo.Organization,
                SignCipher = verification.VriInfo.SignCipher,
                MiOwner = verification.VriInfo.MiOwner,
                VrfDate = verification.VriInfo.VrfDate,
                ValidDate = verification.VriInfo.ValidDate,
                VriType = verification.VriInfo.VriType,
                DocTitle = verification.VriInfo.DocTitle,

                Applicable = goodVrf ? new VerificationWithEtalon.Applicable
                {
                    CertNum = verification.VriInfo.Applicable!.CertNum,
                    SignPass = verification.VriInfo.Applicable.SignPass,
                    SignMi = verification.VriInfo.Applicable.SignMi
                } : null,

                Inapplicable = failedVrf ? new VerificationWithEtalon.Inapplicable
                {
                    NoticeNum = verification.VriInfo.Inapplicable!.NoticeNum
                } : null,
            },
            Means = new VerificationWithEtalon.MeansClass
            {
                Mieta = verification.Means.Mieta!
                    .Select(m => new VerificationWithEtalon.Mietum
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
            Info = new VerificationWithEtalon.InfoClass
            {
                BriefIndicator = verification.Info.BriefIndicator,
                Additional_Info = verification.Info.Additional_Info
            }
        };
    }

    public static VerificationWithSes ToSesVerification(this VerificationResult verification, string vri_id)
    {
        var goodVrf = verification.VriInfo.Applicable != null;
        var failedVrf = verification.VriInfo.Inapplicable != null;

        if (goodVrf == failedVrf)
        {
            throw new Exception("Поверка должна быть либо хорошая, либо неудачная");
        }

        return new VerificationWithSes
        {
            Vri_id = vri_id,
            MiInfo = new VerificationWithSes.MiInfoClass
            {
                SingleMI = new VerificationWithSes.SingleMI
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
            VriInfo = new VerificationWithSes.VriInfoClass
            {
                Organization = verification.VriInfo.Organization,
                SignCipher = verification.VriInfo.SignCipher,
                MiOwner = verification.VriInfo.MiOwner,
                VrfDate = verification.VriInfo.VrfDate,
                ValidDate = verification.VriInfo.ValidDate,
                VriType = verification.VriInfo.VriType,
                DocTitle = verification.VriInfo.DocTitle,

                Applicable = goodVrf ? new VerificationWithSes.Applicable
                {
                    CertNum = verification.VriInfo.Applicable!.CertNum,
                    SignPass = verification.VriInfo.Applicable.SignPass,
                    SignMi = verification.VriInfo.Applicable.SignMi
                } : null,

                Inapplicable = failedVrf ? new VerificationWithSes.Inapplicable
                {
                    NoticeNum = verification.VriInfo.Inapplicable!.NoticeNum
                } : null,
            },
            Means = new VerificationWithSes.MeansClass
            {
                Ses = verification.Means.Ses!
                    .Select(m => new VerificationWithSes.Sample
                    {
                        Number = m.Number,
                        Title = m.Title,
                        SeURL = m.SeURL,
                        ManufactureYear = m.ManufactureYear
                    }).ToList()
            },
            Info = new VerificationWithSes.InfoClass
            {
                BriefIndicator = verification.Info.BriefIndicator,
                Additional_Info = verification.Info.Additional_Info
            }
        };
    }
}
