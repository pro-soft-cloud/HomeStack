using HomeStack.Api.Converters;
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
	.AddControllers()
	.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new IPAddressJsonConverter()); })
	;

var app = builder.Build();
var logger = app.Services
	.GetRequiredService<ILoggerFactory>()
	.CreateLogger("HomeStack.Api");

logger.LogInformation("Configuring dependencies for: HomeStack.");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseHomeStackCore(logger);

logger.LogInformation("HomeStack.Api is ready for requests.");

await app.RunAsync();
