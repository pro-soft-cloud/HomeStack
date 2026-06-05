using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace HomeStack.Database.Postgres;

/// <summary>
/// Class ServiceCollectionExtensions.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds the postgres database.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <returns>IServiceCollection.</returns>
	public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<PostgresDbContext>(options => options.UseNpgsql(connectionString));
		services.AddScoped<HomeStackDbContext, PostgresDbContext>();

		return services;
	}
}
