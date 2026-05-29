using Microsoft.EntityFrameworkCore;

namespace ProSoft.HomeStack.Database.Postgres;

public sealed class PostgresDbContext(DbContextOptions<PostgresDbContext> options)
	: HomeStackDbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder); // shared config aus Database-Projekt

		// Hier nur Postgres-spezifische Overrides, z.B.:
		// modelBuilder.HasPostgresExtension("uuid-ossp");
	}
}
