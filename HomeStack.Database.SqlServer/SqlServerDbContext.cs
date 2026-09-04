using Microsoft.EntityFrameworkCore;

namespace HomeStack.Database.SqlServer;

public sealed class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : HomeStackDbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Just MsSqlServer specific Overrides
	}
}
