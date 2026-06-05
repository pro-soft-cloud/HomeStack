using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HomeStack.Core.Default;

namespace HomeStack.Database.Postgres;

internal sealed class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
	public PostgresDbContext CreateDbContext(string[] args)
	{
		var options = new DbContextOptionsBuilder<PostgresDbContext>()
			.UseNpgsql(Environment.GetEnvironmentVariable(OptionVariable.DatabaseConnectionStringPostgres))
			.Options;

		return new PostgresDbContext(options);
	}
}