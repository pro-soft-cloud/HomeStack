using HomeStack.Api.Converters;
using HomeStack.Api.ServiceConfigurations;
using HomeStack.Core.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure WebApi
builder
	.ConfigureSerilog()
	.ConfigureOpenTelemetry()
	;

// Add services to the container.
builder.Services
	.AddHomeStackCore()
	.ConfigureHttpJsonOptions(options =>
	{
		options.SerializerOptions.Converters.Add(new IPAddressJsonConverter());
	})
	.AddAuthorization()
	;

builder.Services.AddOpenApi();

var app = builder.Build();
var logger = app.Services
	.GetRequiredService<ILoggerFactory>()
	.CreateLogger("HomeStack.Api");

logger.LogInformation("Configuring dependencies for: HomeStack.");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapHomeStackEndpoints();

app.UseHomeStackCore(logger);

app.MapOpenApi();
app.MapScalarApiReference();

logger.LogInformation("HomeStack.Api is ready for requests.");

await app.RunAsync();
