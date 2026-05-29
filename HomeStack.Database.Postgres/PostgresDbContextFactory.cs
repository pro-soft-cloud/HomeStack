using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProSoft.HomeStack.Core.Default;

namespace ProSoft.HomeStack.Database.Postgres;

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