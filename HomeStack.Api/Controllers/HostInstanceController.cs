using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using HomeStack.Api.Extensions;
using HomeStack.Api.Mappers;
using HomeStack.Api.Models.Request.HostInstance;
using HomeStack.Api.Models.Response;
using HomeStack.Core.Models;
using HomeStack.Logic.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HomeStack.Api.Controllers;

[ApiController]
[Route("v1/hostinstances")]
[Produces("application/json")]
public sealed class HostInstanceController : ControllerBase
{
	private readonly ILogger<HostInstanceController> _logger;
	private readonly IHostInstanceManager _hostInstanceManager;

	public HostInstanceController(ILogger<HostInstanceController> logger, IHostInstanceManager hostInstanceManager)
	{
		_logger = logger;
		_hostInstanceManager = hostInstanceManager;
	}

	[HttpGet("")]
	public async Task<ActionResult<List<HostInstanceModel>>> GetAllAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await _hostInstanceManager.GetAllHostInstancesAsync(pageNumber, pageSize, cancellationToken);

			return result
				.Map(hostInstances => hostInstances.Select(h => h.ToApiModel()).ToList())
				.ToPagedResult(result.PagedInfo)
				.ToActionResult(this);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving host instances.");

			return Result<List<HostInstanceModel>>.CriticalError("Due to a technical error, no data could be retrieved.").ToActionResult(this);
		}
	}

	[HttpGet("{systemId}", Name = "GetBySystemId")]
	public async Task<ActionResult<HostInstanceModel>> GetBySystemIdAsync([FromRoute] Guid systemId, CancellationToken cancellationToken = default)
	{
		var result = await _hostInstanceManager.GetBySystemIdAsync(systemId, cancellationToken);

		return result
			.Map(hostInstance => hostInstance!.ToApiModel())
			.ToActionResult(this);
	}

	[HttpPut("{systemId}")]
	public async Task<ActionResult<HostInstanceModel>> UpdateAsync([FromRoute] Guid systemId, [FromBody] UpdateHostInstanceModel request, CancellationToken cancellationToken = default)
	{
		if (systemId != request.SystemId)
		{
			var validationError = new ValidationError
			{
				Identifier = nameof(request.SystemId),
				ErrorMessage = "Route systemId and body SystemId must match."
			};

			return Result<HostInstanceModel>.Invalid([validationError]).ToActionResult(this);
		}

		var coreModel = request.ToCoreModel();

		coreModel.LastUpdatedBy = User?.Identity?.Name ?? "system";
		coreModel.LastUpdatedAt = DateTimeOffset.UtcNow;

		var result = await _hostInstanceManager.UpdateAsync(coreModel, cancellationToken);

		return result
			.Map(hostInstance => hostInstance.ToApiModel())
			.ToActionResult(this);
	}

	[HttpPost("")]
	public async Task<ActionResult<HostInstanceModel>> CreateAsync([FromBody] CreateHostInstanceModel request, CancellationToken cancellationToken = default)
	{
		var coreModel = request.ToCoreModel();

		coreModel.CreatedBy = User?.Identity?.Name ?? "system";

		var result = await _hostInstanceManager.AddAsync(coreModel, cancellationToken);

		if (result.Status != ResultStatus.Created)
		{
			return result.Map(hostInstance => hostInstance.ToApiModel()).ToActionResult(this);
		}

		var apiModel = result.Value.ToApiModel();

		return CreatedAtRoute("GetBySystemId", new { systemId = apiModel.SystemId }, apiModel);
	}
}
