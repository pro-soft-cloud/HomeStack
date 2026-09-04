using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeStack.Core.Options;
using HomeStack.Database.Postgres;
using HomeStack.Database.SqlServer;

namespace HomeStack.Database.Infrastructure;

public static class DependencyResolver
{
	public static IServiceCollection AddHomeStackDatabase(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		switch (HomeStackOptions.DatabaseEngine)
		{
			case DatabaseEngine.Postgres:
				services.AddDbContext<PostgresDbContext>((sp, opts) => opts.UseNpgsql(HomeStackOptions.ConnectionString));
				break;

			// MsSqlServer is default
			default:
				services.AddDbContext<SqlServerDbContext>((sp, opts) => opts.UseSqlServer(HomeStackOptions.ConnectionString));
				break;
		}

		services.AddScoped<HomeStackDbContext>(sp =>
		{
			return HomeStackOptions.DatabaseEngine switch
			{
				DatabaseEngine.Postgres => sp.GetRequiredService<PostgresDbContext>(),
				DatabaseEngine.MsSqlServer => sp.GetRequiredService<SqlServerDbContext>(),
				_ => throw new NotSupportedException($"Database engine '{HomeStackOptions.DatabaseEngine}' is not supported.")
			};
		});

		return services;
	}

	public static IHost UseHomeStackDatabase(this IHost host, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(logger);

		logger.LogInformation("Configuring dependencies for: HomeStack.Database.");
		host.MigrateDatabase<HomeStackDbContext>(logger);

		return host;
	}
}
