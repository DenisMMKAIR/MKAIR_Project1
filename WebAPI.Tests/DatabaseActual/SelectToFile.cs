using System.Text.RegularExpressions;
using Infrastructure.FGIS.Database;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database.SupportTypes;
using ProjApp.Normalizers;

namespace WebAPI.Tests.DatabaseActual;

public partial class SelectToFile : DatabaseActualFixture
{
    // [Test]
    public async Task DeviceTypeNumberToMethods()
    {
        await using var scope = ScopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FGISDatabase>();
        CancellationToken? cancellationToken = CancellationToken.None;

        var n = VerificationMethodAliasNormalizer.Instance;
        string Normalize(string s)
        {
            return $"'{n.Normalize(s)}'";
        }

        var result = db.VerificationsWithEtalon
            .Select(v => new
            {
                TypeNumber = v.MiInfo.SingleMI.MitypeNumber,
                TypeTitle = v.MiInfo.SingleMI.MitypeTitle.ToLower().Trim(),
                TypeNotation = v.MiInfo.SingleMI.MitypeType.ToLower().Trim(),
                DeviceSerial = v.MiInfo.SingleMI.ManufactureNum,
                DeviceModification = v.MiInfo.SingleMI.Modification,
                Date = v.VriInfo.VrfDate,
                VrfName = v.VriInfo.DocTitle,
            })
            .AsEnumerable()
            .GroupBy(v => v.TypeNumber)
            .Select(g => new TypeDTO(
                TypeNumber: g.Key,
                TypeInfos: g.GroupBy(dto => dto.TypeTitle)
                    .Select(g => new TypeInfoDTO(
                        TypeTitle: g.Key,
                        TypeNotation: g.Select(dto => dto.TypeNotation)
                            .Distinct()
                            .OrderBy(n => n.Length)
                            .ThenBy(n => n)
                            .ToArray()
                    ))
                    .ToArray(),
                Vrf: g.Select(dto => new
                {
                    dto.VrfName,
                    VrfNameNormal = Normalize(dto.VrfName),
                    dto.DeviceSerial,
                    dto.DeviceModification,
                    dto.Date,
                })
                .GroupBy(dto => dto.VrfNameNormal)
                .Select(g => new VrfDTO(
                    NormalName: g.Key,
                    Names: g.Select(dto => dto.VrfName)
                        .Distinct()
                        .OrderBy(n => n.Length)
                        .ThenBy(n => n)
                        .ToArray(),
                    Serials: g.Select(dto => dto.DeviceSerial)
                        .Distinct()
                        .OrderBy(s => s.Length)
                        .ThenBy(s => s)
                        .ToArray(),
                    Modifications: g.Select(dto => dto.DeviceModification)
                        .DistinctBy(m => Normalize(m))
                        .OrderBy(m => m.Length)
                        .ThenBy(m => m)
                        .ToArray(),
                    Dates: g.Select(dto => (YearMonth)dto.Date)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToArray()
                ))
                .OrderBy(dto => dto.NormalName.Length)
                .ThenBy(dto => dto.NormalName)
                .ToArray()
            ))
            .OrderByDescending(dto => dto.Vrf.Length)
            .ToArray();

        WriteFile("Все. Методики по номеру типа.txt", result);
        WriteFile("Больше 1 методики. Методики по номеру типа.txt", result.Where(dto => dto.Vrf.Length > 1));
        WriteFile("Больше 1 описания типа. Методики по номеру типа.txt", result.Where(dto => dto.TypeInfos.Count > 1));
    }

    private static void WriteFile(string fileName, IEnumerable<TypeDTO> result)
    {
        using var fs = File.OpenWrite(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    fileName));

        using var sw = new StreamWriter(fs);

        foreach (var dto in result)
        {
            sw.WriteLine(dto.TypeNumber);
            foreach (var typeInfo in dto.TypeInfos)
            {
                sw.WriteLine($"\t{typeInfo.TypeTitle}");
                sw.Write("\t\t");
                sw.WriteLine(string.Join("\n\t\t", typeInfo.TypeNotation));
            }
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

    private record TypeDTO(string TypeNumber, IReadOnlyList<TypeInfoDTO> TypeInfos, VrfDTO[] Vrf);
    private record TypeInfoDTO(string TypeTitle, IReadOnlyList<string> TypeNotation);
    private record VrfDTO(string NormalName, IReadOnlyList<string> Names, IReadOnlyList<string> Serials, IReadOnlyList<string> Modifications, IReadOnlyList<YearMonth> Dates);
}
