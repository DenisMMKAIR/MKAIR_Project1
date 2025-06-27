using Infrastructure.FGIS.Database;
using Infrastructure.FGISAPI.Client;
using Microsoft.Extensions.DependencyInjection;
using ProjApp.InfrastructureInterfaces;

namespace Infrastructure.FGISAPI;

public static class FGISAPIDependencyInjection
{
    public static void AddFGISAPI(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<FGISDatabase>(builder => FGISDatabaseFactory.Configure(builder));
        serviceCollection.AddSingleton<IFGISAPI, FGISAPIClient>();
    }
}