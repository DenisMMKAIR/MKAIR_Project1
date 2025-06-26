using System.Diagnostics.CodeAnalysis;

namespace ProjApp.Database.Entities;

public class InitialVerificationJob : DatabaseEntity
{
    public required string Date { get; set; }
    public required uint LoadedVerifications { get; set; }
}

