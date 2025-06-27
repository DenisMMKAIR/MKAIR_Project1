namespace Infrastructure.FGIS.Database.Entities;

public record Etalon(
    Guid Id,
    string RegNumber,
    string TypeNumber,
    string TypeTitle,
    string Notation,
    string Modification,
    string ManufactureNum,
    uint ManufactureYear,
    string RankCode,
    string RankTitle,
    string SchemaTitle);
