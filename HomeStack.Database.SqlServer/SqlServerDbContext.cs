using Microsoft.EntityFrameworkCore;

namespace HomeStack.Database.SqlServer;

public sealed class SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
	: HomeStackDbContext(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// SqlServer-spezifische Overrides, z.B.:
		// modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");
	}
}
