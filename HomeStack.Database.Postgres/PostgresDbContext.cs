using Microsoft.EntityFrameworkCore;

namespace HomeStack.Database.Postgres;

public sealed class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : HomeStackDbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Just Postgres specific Overrides
	}
}
