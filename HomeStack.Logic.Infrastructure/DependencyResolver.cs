using HomeStack.Logic.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HomeStack.Logic.Infrastructure;

public static class DependencyResolver
{
	public static IServiceCollection AddHomeStackLogic(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		services.AddScoped<IHostInstanceManager, HostInstanceManager>();

		return services;
	}

	public static IHost UseHomeStackLogic(this IHost host, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(logger);

		logger.LogInformation("Configuring dependencies for: HomeStack.Logic.");

		return host;
	}
}
