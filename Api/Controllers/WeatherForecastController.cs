using Microsoft.AspNetCore.Mvc;
using ProSoft.Result;

namespace ProSoft.HomeStack.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
	private static readonly string[] Summaries =
	[
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	];

	[HttpGet(Name = "GetWeatherForecast")]
	public ActionResult<Result<IEnumerable<WeatherForecast>>> Get()
	{
		var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
		{
			Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		})
		.ToArray();

		var result = new Result<IEnumerable<WeatherForecast>>(data, ResultStatus.Success);

		return new UnprocessableEntityObjectResult(result);
	}
}
