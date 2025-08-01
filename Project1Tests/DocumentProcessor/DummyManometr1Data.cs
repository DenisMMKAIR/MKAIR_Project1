using ProjApp.Database.EntitiesStatic;
using ProjApp.ProtocolForms;

namespace Project1Tests.DocumentProcessor;

public static class DummyManometr1Data
{
    public static ManometrForm ShortDeviceInfo()
    {
        return CreateForm(
            deviceInfo: "Манометры избыточного давления; 232.30.160"
        );
    }

    public static ManometrForm LongDeviceInfo()
    {
        return CreateForm();
    }

    private static ManometrForm CreateForm(string? deviceInfo = null)
    {
        var date = new DateOnly(2024, 05, 06);

        return new()
        {
            Address = MKAIRInfo.GetAddress(date),
            ProtocolNumber = "05/107/24",
            DeviceInfo = deviceInfo ?? "Манометры избыточного давления, вакуумметры и мановакуумметры показывающие МП-Уф, ВП-Уф, МВП-Уф, ДМ8010-Уф, ДВ8010-Уф, ДА8010-Уф; 232.30.160",
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
            Checkups = new Dictionary<string, string>
            {
                ["Результат внешнего осмотра"] = "5.1",
                ["Результат опробования"] = "5.2.3",
                ["Определение основной погрешности"] = "5.3"
            },
            MeasurementUnit = "МПа",
            DeviceValues = [[1, 2, 3, 4, 5, 6, 7, 8], [9, 10, 11, 12, 13, 14, 15, 16]],
            EtalonValues = [[17, 18, 19, 20, 21, 22, 23, 24], [25, 26, 27, 28, 29, 30, 31, 32]],
            ActualError = [[33, 34, 35, 36, 37, 38, 39, 40], [41, 42, 43, 44, 45, 46, 47, 48]],
            ValidError = 1.2,
            ActualVariation = [49, 50, 51, 52, 53, 54, 55, 56],
            VerificationDate = date,
            Worker = "Большаков С.Н."
        };
    }
}
