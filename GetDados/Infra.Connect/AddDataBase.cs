using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Connect;

public static class AddDataBase
{
    public static IServiceCollection AdicionarSqlite(this IServiceCollection services)
    {
        services.AddDbContext<GetDadosContext>(x 
            => x.UseSqlite($"Data Source=GetDados.DB"));

        return services;
    }
}
