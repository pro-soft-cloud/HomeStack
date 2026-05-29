namespace ProSoft.HomeStack.Core.Models.Contracts;

/// <summary>
/// Interface IBaseObject
/// </summary>
public interface IBaseObject
{
	/// <summary>
	/// Gets or sets the system identifier.
	/// </summary>
	/// <value>The system identifier.</value>
	Guid SystemId { get; set; }

	/// <summary>
	/// Gets or sets the valid from.
	/// </summary>
	/// <value>The valid from.</value>
	DateTimeOffset ValidFrom { get; set; }

	/// <summary>
	/// Gets or sets the valid to.
	/// </summary>
	/// <value>The valid to.</value>
	DateTimeOffset ValidTo { get; set; }

	/// <summary>
	/// Gets or sets the created at.
	/// </summary>
	/// <value>The created at.</value>
	DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the created by.
	/// </summary>
	/// <value>The created by.</value>
	string CreatedBy { get; set; }

	/// <summary>
	/// Gets or sets the last updated at.
	/// </summary>
	/// <value>The last updated at.</value>
	DateTimeOffset? LastUpdatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last updated by.
	/// </summary>
	/// <value>The last updated by.</value>
	string LastUpdatedBy { get; set; }
}