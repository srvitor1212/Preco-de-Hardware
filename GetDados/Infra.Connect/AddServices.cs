using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Connect;

public static class AddServices
{
    public static IServiceCollection AdicionarServices(this IServiceCollection services)
    {
        services.AddHttpClient<IKabumScrapingService, KabumScrapingService>();

        return services;
    }
}
