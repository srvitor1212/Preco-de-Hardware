using Microsoft.EntityFrameworkCore;

namespace Infra.Data;

public class AppContext
    (DbContextOptions<AppContext> options)
    : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppContext).Assembly);
    }
}
