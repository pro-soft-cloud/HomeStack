using ProSoft.HomeStack.Api.ServiceConfigurations;
using ProSoft.HomeStack.Core.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure WebApi
builder
	.ConfigureSerilog()
	;

// Add services to the container.
builder.Services
	.AddHomeStackLogic()
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

app.UseHomeStackLogic(logger);

logger.LogInformation("HomeStack.Api is ready for requests.");

await app.RunAsync();
