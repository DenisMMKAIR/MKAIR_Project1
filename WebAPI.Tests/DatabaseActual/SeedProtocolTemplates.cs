using Microsoft.Extensions.DependencyInjection;
using ProjApp.Database;

namespace WebAPI.Tests.DatabaseActual;

public class SeedProtocolTemplates : DatabaseActualFixture
{
    [Test]
    public async Task Test1()
    {
        Environment.Exit(0);
        
        using var scope = ScopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ProjDatabase>();
        var protocol = db.ProtocolTemplates.FirstOrDefault(p => p.ProtocolGroup == ProjApp.Database.EntitiesStatic.ProtocolGroup.Манометр1);

        if (protocol == null)
        {
            db.ProtocolTemplates.Add(new() { ProtocolGroup = ProjApp.Database.EntitiesStatic.ProtocolGroup.Манометр1, VerificationGroup = ProjApp.Database.EntitiesStatic.VerificationGroup.Манометры });

            await db.SaveChangesAsync();
        }
    }
}