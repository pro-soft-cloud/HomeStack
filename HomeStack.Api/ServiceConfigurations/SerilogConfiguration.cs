using Serilog;

namespace ProSoft.HomeStack.Api.ServiceConfigurations;

/// <summary>
/// Class SerilogConfiguration.
/// </summary>
internal static class SerilogConfiguration
{
	/// <summary>
	/// Configures the serilog.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>WebApplicationBuilder.</returns>
	internal static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
	{
		builder.Host.UseSerilog
		(
			(hostingContext, logConfig) => logConfig.ReadFrom.Configuration(hostingContext.Configuration)
		);

		return builder;
	}
}
