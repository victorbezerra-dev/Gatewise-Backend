using Microsoft.EntityFrameworkCore;
using GateWise.Core.Entities;

namespace GateWise.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<LabAccessManager> LabAccessManagers => Set<LabAccessManager>();
    public DbSet<AccessGrant> AccessGrants => Set<AccessGrant>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
