namespace HomeStack.Api.Models.Response;

public record HostInstanceModel
(
	Guid SystemId,
	string DisplayName,
	string IP,
	DateTimeOffset ValidFrom,
	DateTimeOffset ValidTo,
	DateTimeOffset CreatedAt,
	string CreatedBy,
	DateTimeOffset? LastUpdatedAt,
	string? LastUpdatedBy
);