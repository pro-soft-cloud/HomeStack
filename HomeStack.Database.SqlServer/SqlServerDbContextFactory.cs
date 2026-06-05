using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using HomeStack.Core.Default;

namespace HomeStack.Database.SqlServer;

internal sealed class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
	public SqlServerDbContext CreateDbContext(string[] args)
	{
		var options = new DbContextOptionsBuilder<SqlServerDbContext>()
			.UseSqlServer(Environment.GetEnvironmentVariable(OptionVariable.DatabaseConnectionStringMsSqlServer))
			.Options;

		return new SqlServerDbContext(options);
	}
}