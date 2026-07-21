using System.Reflection.Metadata.Ecma335;
using HomeStack.Api.Mappers;
using HomeStack.Api.Models.Request.HostInstance;
using HomeStack.Api.Models.Response;
using HomeStack.Core.Models;
using HomeStack.Logic.Contracts;
using Microsoft.AspNetCore.Mvc;
using ProSoft.Result;

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
	public async Task<ActionResult<Result<List<HostInstanceModel>>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var result = await _hostInstanceManager.GetAllHostInstancesAsync(cancellationToken);

		var response = new Result<List<HostInstanceModel>>
		(
			result.Select(r => r.ToApiModel()).ToList(),
			ResultStatus.Success,
			[
				new Message(MessageCategory.Business, MessageType.Hint, "IP ist doppelt vergeben.", "019f6f3c-70a1-723a-87f9-f15c47c2a85d", "Ip")
			]
		);

		return result is not { Count: > 0 }
			? NotFound()
			: new OkObjectResult(response);
	}

	[HttpGet("{systemId}", Name = "GetBySystemId")]
	public async Task<ActionResult<Result<HostInstanceModel>>> GetBySystemIdAsync([FromRoute] Guid systemId, CancellationToken cancellationToken)
	{
		var result = await _hostInstanceManager.GetBySystemIdAsync(systemId, cancellationToken);

		return result == null 
			? NotFound() 
			: new OkObjectResult(result.ToApiModel());
	}

	[HttpPut("{systemId}")]
	public async Task<ActionResult<Result<HostInstance>>> UpdateAsync([FromRoute] Guid systemId, [FromBody] UpdateHostInstanceModel request, CancellationToken cancellationToken)
	{
		if (systemId != request.SystemId)
			return new BadRequestResult();

		var coreModel = request.ToCoreModel();

		coreModel.LastUpdatedBy = User?.Identity?.Name ?? "system";
		coreModel.LastUpdatedAt = DateTimeOffset.UtcNow;

		var result = await _hostInstanceManager.UpdateAsync(coreModel, cancellationToken);

		return result.Data == null
			? NotFound()
			: new OkObjectResult(result.Data.ToApiModel());
	}

	[HttpPost("")]
	public async Task<ActionResult<Result<HostInstance>>> CreateAsync([FromBody] CreateHostInstanceModel request, CancellationToken cancellationToken)
	{
		var coreModel = request.ToCoreModel();

		coreModel.CreatedBy = User?.Identity?.Name ?? "system";

		var result = await _hostInstanceManager.AddAsync(coreModel, cancellationToken);

		var response = new CreatedAtRouteResult("GetBySystemId", new { systemId = result.SystemId }, result);

		return response;
	}
}
