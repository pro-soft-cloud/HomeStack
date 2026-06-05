using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeStack.Database.Infrastructure;
using HomeStack.Logic.Infrastructure;

namespace HomeStack.Core.Infrastructure;

public static class DependencyResolver
{
	public static IServiceCollection AddHomeStackCore(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		services
			.AddHomeStackLogic()
			.AddHomeStackDatabase();

		return services;
	}

	public static IHost UseHomeStackCore(this IHost host, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(logger);

		logger.LogInformation("Configuring dependencies for: HomeStack.Core.");

		host
			.UseHomeStackLogic(logger)
			.UseHomeStackDatabase(logger);

		return host;
	}
}
