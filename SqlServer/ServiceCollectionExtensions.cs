using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ProSoft.HomeStack.Database.SqlServer;

/// <summary>
/// Class ServiceCollectionExtensions.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds the SQL server database.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <returns>IServiceCollection.</returns>
	public static IServiceCollection AddSqlServerDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(connectionString));
		services.AddScoped<HomeStackDbContext, SqlServerDbContext>();

		return services;
	}
}
