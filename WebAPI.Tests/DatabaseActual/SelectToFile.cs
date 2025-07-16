using System.Text.RegularExpressions;
using Infrastructure.FGIS.Database;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database.SupportTypes;

namespace WebAPI.Tests.DatabaseActual;

public class SelectToFile : DatabaseActualFixture
{
    [Test]
    public async Task DeviceTypeNumberToMethods()
    {
        Environment.Exit(0);

        await using var scope = ScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FGISDatabase>();
        CancellationToken? cancellationToken = CancellationToken.None;

        var vrfNameRegex = new Regex("^МИ|^МП|МИ$|МП$| |-|«|»|\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var result = db.Verifications
            .Select(v => new
            {
                TypeNumber = v.MiInfo.SingleMI.MitypeNumber,
                TypeInfo = $"{v.MiInfo.SingleMI.MitypeTitle} {v.MiInfo.SingleMI.MitypeType}",
                DeviceSerial = v.MiInfo.SingleMI.ManufactureNum,
                DeviceModification = v.MiInfo.SingleMI.Modification,
                Date = v.VriInfo.VrfDate,
                VrfName = v.VriInfo.DocTitle,
            })
            .AsEnumerable()
            .GroupBy(v => v.TypeNumber)
            .Select(g =>
            {
                var first = g.First();
                return new
                {
                    TypeNumber = g.Key,
                    first.TypeInfo,
                    Vrf = g.Select(dto => new
                    {
                        dto.VrfName,
                        VrfNameNormal = $"'{vrfNameRegex.Replace(dto.VrfName.ToUpper(), "")}'",
                        dto.DeviceSerial,
                        dto.DeviceModification,
                        dto.Date,
                    })
                    .GroupBy(dto => dto.VrfNameNormal)
                    .OrderBy(g => g.First().VrfNameNormal.Length)
                    .Select(g =>
                    {
                        return new
                        {
                            NormalName = g.Key,
                            Names = g.Select(dto => dto.VrfName)
                                .Distinct()
                                .OrderBy(n => n.Length)
                                .ToArray(),
                            Serials = g.Select(dto => dto.DeviceSerial)
                                .Distinct()
                                .OrderBy(s => s.Length)
                                .ToArray(),
                            Modifications = g.Select(dto => dto.DeviceModification)
                                .Distinct()
                                .OrderBy(m => m.Length)
                                .ToArray(),
                            Dates = g.Select(dto => (YearMonth)dto.Date)
                                .Distinct()
                                .OrderBy(d => d)
                                .ToArray(),
                        };
                    })
                    .ToArray(),
                };
            })
            .OrderByDescending(dto => dto.Vrf.Length)
            .ToArray();

        {
            using var fs = File.OpenWrite(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Все. Методики по номеру типа.txt"));

            using var sw = new StreamWriter(fs);

            foreach (var dto in result)
            {
                sw.WriteLine($"{dto.TypeNumber} {dto.TypeInfo}");
                sw.WriteLine(new string('-', 5));
                foreach (var vrf in dto.Vrf)
                {
                    // sw.WriteLine("\t" + "Обопщение имен методик по " + vrf.NormalName);
                    sw.WriteLine("\t" + "Методики " + string.Join("; ", vrf.Names));
                    sw.WriteLine("\t" + "Серийные " + string.Join("; ", vrf.Serials));
                    sw.WriteLine("\t" + "Модификации " + string.Join("; ", vrf.Modifications));
                    sw.WriteLine("\t" + "Даты " + string.Join("; ", vrf.Dates));
                    sw.WriteLine(new string('-', 5));
                }
                sw.WriteLine(new string('-', 20));
            }
        }

        {
            using var fs = File.OpenWrite(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Больше 1 методики. Методики по номеру типа.txt"));

            using var sw = new StreamWriter(fs);

            foreach (var dto in result.Where(dto => dto.Vrf.Length > 1))
            {
                sw.WriteLine($"{dto.TypeNumber} {dto.TypeInfo}");
                sw.WriteLine(new string('-', 5));
                foreach (var vrf in dto.Vrf)
                {
                    // sw.WriteLine("\t" + "Обопщение имен методик по " + vrf.NormalName);
                    sw.WriteLine("\t" + "Методики " + string.Join("; ", vrf.Names));
                    sw.WriteLine("\t" + "Серийные " + string.Join("; ", vrf.Serials));
                    sw.WriteLine("\t" + "Модификации " + string.Join("; ", vrf.Modifications));
                    sw.WriteLine("\t" + "Даты " + string.Join("; ", vrf.Dates));
                    sw.WriteLine(new string('-', 5));
                }
                sw.WriteLine(new string('-', 20));
            }
        }
    }
}
