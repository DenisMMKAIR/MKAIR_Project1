namespace ProjApp.Database.Entities;

public class Owner : DatabaseEntity
{
    public required string Name { get; set; }
    public required ulong INN { get; set; }
}
