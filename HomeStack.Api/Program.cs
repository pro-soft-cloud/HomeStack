using HomeStack.Api.ServiceConfigurations;
using HomeStack.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure WebApi
builder
	.ConfigureSerilog()
	;

// Add services to the container.
builder.Services
	.AddHomeStackCore()
	.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
var logger = app.Services
	.GetRequiredService<ILoggerFactory>()
	.CreateLogger("HomeStack.Api");

logger.LogInformation("Configuring dependencies for: HomeStack.");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseHomeStackCore(logger);

logger.LogInformation("HomeStack.Api is ready for requests.");

await app.RunAsync();
