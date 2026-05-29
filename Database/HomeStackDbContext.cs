using Microsoft.EntityFrameworkCore;

namespace ProSoft.HomeStack.Database;

public abstract class HomeStackDbContext(DbContextOptions options) : DbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(HomeStackDbContext).Assembly);
	}
}
