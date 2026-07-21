namespace HomeStack.Api.Models.Request.HostInstance;

public record CreateHostInstanceModel
(
	string DisplayName,
	string IP,
	DateTimeOffset? ValidFrom,
	DateTimeOffset? ValidTo
);
