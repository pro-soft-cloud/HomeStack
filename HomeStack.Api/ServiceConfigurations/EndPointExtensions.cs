using HomeStack.Api.Endpoints;

namespace HomeStack.Api.ServiceConfigurations;

public static class EndPointExtensions
{
	internal static IEndpointRouteBuilder MapHomeStackEndpoints(this IEndpointRouteBuilder app)
	{
		app
			.MapHostInstanceEndpoints();

		return app;
	}
}