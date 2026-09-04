using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;

namespace HomeStack.Api.Extensions;

public static class PagedResultExtensions
{
	public static ActionResult<T> ToActionResult<T>(this PagedResult<T> pagedResult, ControllerBase controller)
	{
		return pagedResult.Status switch
		{
			ResultStatus.Ok => controller.Ok(pagedResult), // ganzes PagedResult inkl. PagedInfo
			ResultStatus.NotFound => controller.NotFound(),
			ResultStatus.Invalid => Invalid(pagedResult, controller),
			ResultStatus.CriticalError => controller.StatusCode(StatusCodes.Status500InternalServerError, pagedResult.Errors),
			_ => controller.StatusCode(StatusCodes.Status500InternalServerError, pagedResult.Errors)
		};
	}

	private static ActionResult<T> Invalid<T>(PagedResult<T> pagedResult, ControllerBase controller)
	{
		foreach (var error in pagedResult.ValidationErrors)
		{
			controller.ModelState.AddModelError(error.Identifier, error.ErrorMessage);
		}

		return controller.BadRequest(controller.ModelState);
	}
}