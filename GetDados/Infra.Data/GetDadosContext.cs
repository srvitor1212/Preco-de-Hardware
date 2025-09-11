using Microsoft.EntityFrameworkCore;

namespace Infra.Data;

public class GetDadosContext
    (DbContextOptions<GetDadosContext> options)
    : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GetDadosContext).Assembly);
    }
}
