using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProSoft.HomeStack.Core.Default;

namespace ProSoft.HomeStack.Database.SqlServer;

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