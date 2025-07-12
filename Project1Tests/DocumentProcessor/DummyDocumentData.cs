using ProjApp.Database.Entities;
using ProjApp.Database.EntitiesStatic;

namespace Project1Tests.DocumentProcessor;

public static class DummyDocumentData
{
    public static Manometr1Verification ManometrData1Eta()
    {
        var verDate = new DateOnly(2024, 05, 06);
        var devSerial = "00619";
        var deviceModification = "232.30.160";
        var deviceTypeNumber = "55984-13";
        var manufactureYear = 2013u;
        var deviceTypeTitle = "Манометры избыточного давления, вакуумметры и мановакуумметры показывающие";
        var deviceTypeNotation = "МП-Уф, ВП-Уф, МВП-Уф, ДМ8010-Уф, ДВ8010-Уф, ДА8010-Уф";
        var deviceTypeInfo = $"{deviceTypeTitle} {deviceTypeNotation}";

        var data = new Manometr1Verification
        {
            Address = "1 Ханты-Мансийский автономный округ - Югра, г.о. Нижневартовск, г Нижневартовск, ул Индустриальная, зд. 32, стр. 1, кабинет 14",
            ProtocolNumber = "05/107/24",
            DeviceTypeName = deviceTypeInfo,
            DeviceModification = deviceModification,
            DeviceTypeNumber = deviceTypeNumber,
            DeviceSerial = devSerial,
            ManufactureYear = manufactureYear,
            Owner = "ООО \"РИ-ИНВЕСТ\"",
            OwnerINN = 7705551779,
            VerificationsInfo = "МИ 2124-90 «ГСИ. Манометры, вакуумметры, мановакуумметры, напоромеры, тягомеры и тягонапоромеры показывающие и самопишущие. Методика поверки»",
            EtalonsInfo = "73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021",
            Temperature = 23.1f,
            Humidity = 41,
            Pressure = "95,5 кПа",
            VerificationVisualCheckup = "5.1",
            VerificationResultCheckup = "5.2.3",
            VerificationAccuracyCheckup = "5.3",
            VerificationDate = verDate,
            Worker = "Большаков С.Н.",

            // Support fields
            VerificationGroup = VerificationGroup.Манометры,
            Location = DeviceLocation.ГПНЯмал,
            VerifiedUntilDate = verDate.AddYears(2).AddDays(-1),

            // Table values
            MeasurementMin = 0,
            MeasurementMax = 150,
            MeasurementUnit = "МПа",
            ValidError = 1,

            DeviceValues = [[1, 2, 3, 4, 5, 6, 7, 8], [9, 10, 11, 12, 13, 14, 15, 16]],
            EtalonValues = [[17, 18, 19, 20, 21, 22, 23, 24], [25, 26, 27, 28, 29, 30, 31, 32]],
            ActualError = [[33, 34, 35, 36, 37, 38, 39, 40], [41, 42, 43, 44, 45, 46, 47, 48]],
            ActualVariation = [49, 50, 51, 52, 53, 54, 55, 56],

            Device = new Device
            {
                DeviceTypeNumber = deviceTypeNumber,
                Serial = devSerial,
                Modification = deviceModification,
                ManufacturedYear = manufactureYear,

                DeviceType = new DeviceType
                {
                    Number = deviceTypeNumber,
                    Title = deviceTypeTitle,
                    Notation = deviceTypeNotation,

                    VerificationMethod = new VerificationMethod
                    {
                        Aliases = ["МИ 2124-90"],
                        Checkups = new Dictionary<VerificationMethodCheckups, string>{
                            { VerificationMethodCheckups.visual, "5.1" },
                            { VerificationMethodCheckups.result, "5.2.3" },
                            { VerificationMethodCheckups.accuracy, "5.3" } },
                        Description = "",

                        ProtocolTemplate = new ProtocolTemplate
                        {
                            ProtocolGroup = ProtocolGroup.Манометр1,
                            VerificationGroup = VerificationGroup.Манометры,
                        }
                    }
                },
            },
        };

        return data;
    }

    public static Manometr1Verification ManometrData2Eta()
    {
        var verDate = new DateOnly(2024, 05, 06);
        var devSerial = "00619";
        var deviceModification = "232.30.160";
        var deviceTypeNumber = "55984-13";
        var manufactureYear = 2013u;
        var deviceTypeTitle = "Манометры избыточного давления, вакуумметры и мановакуумметры показывающие";
        var deviceTypeNotation = "МП-Уф, ВП-Уф, МВП-Уф, ДМ8010-Уф, ДВ8010-Уф, ДА8010-Уф";
        var deviceTypeInfo = $"{deviceTypeTitle} {deviceTypeNotation}";

        var data = new Manometr1Verification
        {
            Address = "1 Ханты-Мансийский автономный округ - Югра, г.о. Нижневартовск, г Нижневартовск, ул Индустриальная, зд. 32, стр. 1, кабинет 14",
            ProtocolNumber = "05/107/24",
            DeviceTypeName = deviceTypeInfo,
            DeviceModification = deviceModification,
            DeviceTypeNumber = deviceTypeNumber,
            DeviceSerial = devSerial,
            ManufactureYear = manufactureYear,
            Owner = "ООО \"РИ-ИНВЕСТ\"",
            OwnerINN = 7705551779,
            VerificationsInfo = "МИ 2124-90 «ГСИ. Манометры, вакуумметры, мановакуумметры, напоромеры, тягомеры и тягонапоромеры показывающие и самопишущие. Методика поверки»",
            EtalonsInfo = "73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021; 73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021",
            Temperature = 23.1f,
            Humidity = 41,
            Pressure = "95,5 кПа",
            VerificationVisualCheckup = "5.1",
            VerificationResultCheckup = "5.2.3",
            VerificationAccuracyCheckup = "5.3",
            VerificationDate = verDate,
            Worker = "Большаков С.Н.",

            // Support fields
            VerificationGroup = VerificationGroup.Манометры,
            Location = DeviceLocation.ГПНЯмал,
            VerifiedUntilDate = verDate.AddYears(2).AddDays(-1),

            // Table values
            MeasurementMin = 0,
            MeasurementMax = 150,
            MeasurementUnit = "МПа",
            ValidError = 1,

            DeviceValues = [[1, 2, 3, 4, 5, 6, 7, 8], [9, 10, 11, 12, 13, 14, 15, 16]],
            EtalonValues = [[17, 18, 19, 20, 21, 22, 23, 24], [25, 26, 27, 28, 29, 30, 31, 32]],
            ActualError = [[33, 34, 35, 36, 37, 38, 39, 40], [41, 42, 43, 44, 45, 46, 47, 48]],
            ActualVariation = [49, 50, 51, 52, 53, 54, 55, 56],

            Device = new Device
            {
                DeviceTypeNumber = deviceTypeNumber,
                Serial = devSerial,
                Modification = deviceModification,
                ManufacturedYear = manufactureYear,

                DeviceType = new DeviceType
                {
                    Number = deviceTypeNumber,
                    Title = deviceTypeTitle,
                    Notation = deviceTypeNotation,

                    VerificationMethod = new VerificationMethod
                    {
                        Aliases = ["МИ 2124-90"],
                        Checkups = new Dictionary<VerificationMethodCheckups, string>{
                            { VerificationMethodCheckups.visual, "5.1" },
                            { VerificationMethodCheckups.result, "5.2.3" },
                            { VerificationMethodCheckups.accuracy, "5.3" } },
                        Description = "",

                        ProtocolTemplate = new ProtocolTemplate
                        {
                            ProtocolGroup = ProtocolGroup.Манометр1,
                            VerificationGroup = VerificationGroup.Манометры,
                        }
                    }
                },
            },
        };

        return data;
    }
}
