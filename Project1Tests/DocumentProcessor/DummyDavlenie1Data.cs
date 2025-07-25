using ProjApp.Database.EntitiesStatic;
using ProjApp.ProtocolForms;

namespace Project1Tests.DocumentProcessor;

public static class DummyDavlenie1Data
{
    public static DavlenieForm DavlenieData1Eta()
    {
        var date = new DateOnly(2025, 01, 02);

        return new DavlenieForm
        {
            Address = MKAIRInfo.GetAddress(date),
            ProtocolNumber = "05/107/24",
            DeviceInfo = "Манометры избыточного давления, вакуумметры и мановакуумметры показывающие МП-Уф, ВП-Уф, МВП-Уф, ДМ8010-Уф, ДВ8010-Уф, ДА8010-Уф; 232.30.160",
            DeviceTypeNumber = "55984-13",
            DeviceSerial = "00619",
            ManufactureYear = 2013u,
            Owner = "ООО \"РИ-ИНВЕСТ\"",
            OwnerINN = 7705551779,
            VerificationsInfo = "МИ 2124-90 «ГСИ. Манометры, вакуумметры, мановакуумметры, напоромеры, тягомеры и тягонапоромеры показывающие и самопишущие. Методика поверки»",
            EtalonsInfo = "73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021",
            Temperature = 23.1,
            Humidity = 0.41,
            Pressure = "95,5 кПа",
            VisualCheckup = "5.1",
            TestCheckup = "5.2.3",
            AccuracyCalculation = "5.3",
            MeasurementUnit = "МПа",
            PressureInputs = [1, 2, 3, 4, 5],
            EtalonValues = [6, 7, 8, 9, 10],
            DeviceValues = [[11, 12, 13, 14, 15], [16, 17, 18, 19, 20]],
            ActualError = [[21, 22, 23, 24, 25], [26, 27, 28, 29, 30]],
            ValidError = 0.2,
            Variations = [31, 32, 33, 34, 35],
            VerificationDate = date,
            Worker = "Большаков С.Н."
        };
    }

    public static DavlenieForm DavlenieData2Eta()
    {
        var date = new DateOnly(2025, 01, 02);

        return new DavlenieForm
        {
            Address = MKAIRInfo.GetAddress(date),
            ProtocolNumber = "05/107/24",
            DeviceInfo = "Манометры избыточного давления, вакуумметры и мановакуумметры показывающие МП-Уф, ВП-Уф, МВП-Уф, ДМ8010-Уф, ДВ8010-Уф, ДА8010-Уф; 232.30.160",
            DeviceTypeNumber = "55984-13",
            DeviceSerial = "00619",
            ManufactureYear = 2013u,
            Owner = "ООО \"РИ-ИНВЕСТ\"",
            OwnerINN = 7705551779,
            VerificationsInfo = "МИ 2124-90 «ГСИ. Манометры, вакуумметры, мановакуумметры, напоромеры, тягомеры и тягонапоромеры показывающие и самопишущие. Методика поверки»",
            EtalonsInfo = "73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021; 73828.19.1Р.00156416; 73828-19; Калибраторы многофункциональные; ЭЛМЕТРО-Паскаль-03, Паскаль-03 нет модификации; 0050; 2020 1Р; Эталон 1-го разряда; Приказ Росстандарта от 01.10.2018 №2091; свидетельство о поверке № 30289/2020 от 04.12.2020; действительно до 03.12.2021",
            Temperature = 23.1,
            Humidity = 0.41,
            Pressure = "95,5 кПа",
            VisualCheckup = "5.1",
            TestCheckup = "5.2.3",
            AccuracyCalculation = "5.3",
            MeasurementUnit = "МПа",
            PressureInputs = [1, 2, 3, 4, 5],
            EtalonValues = [6, 7, 8, 9, 10],
            DeviceValues = [[11, 12, 13, 14, 15], [16, 17, 18, 19, 20]],
            ActualError = [[21, 22, 23, 24, 25], [26, 27, 28, 29, 30]],
            ValidError = 0.2,
            Variations = [31, 32, 33, 34, 35],
            VerificationDate = date,
            Worker = "Большаков С.Н."
        };
    }
}
