using System.Security.Claims;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using HomeStack.Api.Mappers;
using HomeStack.Api.Models.Request.HostInstance;
using HomeStack.Api.Models.Response;
using HomeStack.Logic.Contracts;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace HomeStack.Api.Endpoints;

public static class HostInstanceEndpoints
{
	internal static IEndpointRouteBuilder MapHostInstanceEndpoints(this IEndpointRouteBuilder app)
	{
		var group = app
			.MapGroup("/v1/hostinstances")
			.WithTags("HostInstances");

		group.MapGet("", GetAllAsync);
		group.MapGet("/{systemId:guid}", GetBySystemIdAsync).WithName("GetBySystemId");
		group.MapPut("/{systemId:guid}", UpdateAsync);
		group.MapPost("", CreateAsync);
		group.MapDelete("/{systemId:guid}", DeleteAsync);

		return app;
	}

	private static async Task<IResult> GetAllAsync(
		IHostInstanceManager hostInstanceManager,
		ILogger<Program> logger,
		int pageNumber = 1,
		int pageSize = 20,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await hostInstanceManager.GetAllHostInstancesAsync(pageNumber, pageSize, cancellationToken);

			return result
				.Map(hostInstances => hostInstances.Select(h => h.ToApiModel()).ToList())
				.ToPagedResult(result.PagedInfo)
				.ToMinimalApiResult();
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while retrieving host instances.");
			return Result<List<HostInstanceModel>>.CriticalError("Due to a technical error, no data could be retrieved.").ToMinimalApiResult();
		}
	}

	private static async Task<IResult> GetBySystemIdAsync(
		Guid systemId,
		IHostInstanceManager hostInstanceManager,
		CancellationToken cancellationToken = default)
	{
		var result = await hostInstanceManager.GetBySystemIdAsync(systemId, cancellationToken);

		return result
			.Map(hostInstance => hostInstance!.ToApiModel())
			.ToMinimalApiResult();
	}

	private static async Task<IResult> UpdateAsync(
		Guid systemId,
		UpdateHostInstanceModel request,
		ClaimsPrincipal user,
		IHostInstanceManager hostInstanceManager,
		CancellationToken cancellationToken = default)
	{
		if (systemId != request.SystemId)
		{
			var validationError = new ValidationError
			{
				Identifier = nameof(request.SystemId),
				ErrorMessage = "Route systemId and body SystemId must match."
			};

			return Result<HostInstanceModel>.Invalid(validationError).ToMinimalApiResult();
		}

		var coreModel = request.ToCoreModel();
		coreModel.LastUpdatedBy = user.Identity?.Name ?? "system";
		coreModel.LastUpdatedAt = DateTimeOffset.UtcNow;

		var result = await hostInstanceManager.UpdateAsync(coreModel, cancellationToken);

		return result
			.Map(hostInstance => hostInstance.ToApiModel())
			.ToMinimalApiResult();
	}

	private static async Task<IResult> CreateAsync(
		CreateHostInstanceModel request,
		ClaimsPrincipal user,
		IHostInstanceManager hostInstanceManager,
		LinkGenerator linkGenerator,
		HttpContext httpContext,
		CancellationToken cancellationToken = default)
	{
		var coreModel = request.ToCoreModel();
		coreModel.CreatedBy = user.Identity?.Name ?? "system";

		var result = await hostInstanceManager.AddAsync(coreModel, cancellationToken);

		if (result.Status != ResultStatus.Created)
		{
			return result.Map(hostInstance => hostInstance.ToApiModel()).ToMinimalApiResult();
		}

		var apiModel = result.Value.ToApiModel();
		var location = linkGenerator.GetPathByName(httpContext, "GetBySystemId", new { systemId = apiModel.SystemId });

		return TypedResults.Created(location, apiModel);
	}

	private static async Task<IResult> DeleteAsync(
		Guid systemId,
		IHostInstanceManager hostInstanceManager,
		CancellationToken cancellationToken = default)
	{
		var result = await hostInstanceManager.DeleteAsync(systemId, cancellationToken);
		
		return result.ToMinimalApiResult();
	}
}