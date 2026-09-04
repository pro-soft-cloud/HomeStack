namespace HomeStack.Api.Models.Request.HostInstance;

public record UpdateHostInstanceModel
(
	Guid SystemId,
	string DisplayName,
	string IP,
	DateTimeOffset ValidFrom,
	DateTimeOffset ValidTo
);
