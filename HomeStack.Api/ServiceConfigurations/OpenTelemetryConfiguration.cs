using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HomeStack.Api.ServiceConfigurations;

internal static class OpenTelemetryConfiguration
{
	internal static WebApplicationBuilder ConfigureOpenTelemetry(this WebApplicationBuilder builder)
	{
		var otlpEndpoint = builder.Configuration["OpenTelemetry:Endpoint"]!;
		var headers = $"X-Seq-ApiKey={builder.Configuration["OpenTelemetry:ApiKey"]}";

		builder.Services.AddOpenTelemetry()
			.ConfigureResource(r => r.AddService(
				builder.Configuration["Serilog:Properties:Application"] ?? "HomeStack.Api"))
			.WithTracing(tracing => tracing
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddEntityFrameworkCoreInstrumentation()
				.AddOtlpExporter(o =>
				{
					o.Endpoint = new Uri($"{otlpEndpoint}/v1/traces");
					o.Protocol = OtlpExportProtocol.HttpProtobuf;
					o.Headers = headers;
				}))
			.WithMetrics(metrics => metrics
				.AddAspNetCoreInstrumentation()
				.AddHttpClientInstrumentation()
				.AddRuntimeInstrumentation()
				.AddOtlpExporter(o =>
				{
					o.Endpoint = new Uri($"{otlpEndpoint}/v1/metrics");
					o.Protocol = OtlpExportProtocol.HttpProtobuf;
					o.Headers = headers;
				}));

		return builder;
	}
}