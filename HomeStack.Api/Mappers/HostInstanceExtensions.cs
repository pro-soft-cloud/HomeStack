using System.Net;
using HomeStack.Api.Models.Request.HostInstance;
using HomeStack.Api.Models.Response;
using HomeStack.Core.Models;

namespace HomeStack.Api.Mappers;

internal static class HostInstanceExtensions
{
	internal static HostInstance ToCoreModel(this CreateHostInstanceModel request)
	{
		return new HostInstance
		{
			DisplayName = request.DisplayName,
			IP = IPAddress.Parse(request.IP),
			SystemId = Guid.CreateVersion7(),
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedBy = "system",
			ValidFrom = request.ValidFrom ?? Core.Default.Date.MinDate,
			ValidTo = request.ValidTo ?? Core.Default.Date.MaxDate
		};
	}

	internal static HostInstance ToCoreModel(this UpdateHostInstanceModel request)
	{
		return new HostInstance
		{
			SystemId = request.SystemId,
			DisplayName = request.DisplayName,
			IP = IPAddress.Parse(request.IP),
			CreatedAt = DateTimeOffset.UtcNow,
			CreatedBy = "system",
			ValidFrom = request.ValidFrom,
			ValidTo = request.ValidTo
		};
	}

	internal static HostInstanceModel ToApiModel(this HostInstance coreModel)
	{
		return new HostInstanceModel
		(
			coreModel.SystemId,
			coreModel.DisplayName,
			coreModel.IP.ToString(),
			coreModel.ValidFrom,
			coreModel.ValidTo,
			coreModel.CreatedAt,
			coreModel.CreatedBy,
			coreModel.LastUpdatedAt,
			coreModel.LastUpdatedBy
		);
	}
}
