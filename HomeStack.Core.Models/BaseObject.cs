using System.Diagnostics.CodeAnalysis;

namespace HomeStack.Core.Models;

/// <summary>
/// Class BaseObject.
/// </summary>
public abstract class BaseObject
{
	/// <summary>
	/// Gets or sets the system identifier.
	/// </summary>
	/// <value>The system identifier.</value>
	public Guid SystemId { get; set; }

	/// <summary>
	/// Gets or sets the valid from.
	/// </summary>
	/// <value>The valid from.</value>
	public DateTimeOffset ValidFrom { get; set; }

	/// <summary>
	/// Gets or sets the valid to.
	/// </summary>
	/// <value>The valid to.</value>
	public DateTimeOffset ValidTo { get; set; }

	/// <summary>
	/// Gets or sets the created at.
	/// </summary>
	/// <value>The created at.</value>
	public DateTimeOffset CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the created by.
	/// </summary>
	/// <value>The created by.</value>
	public string CreatedBy { get; set; }

	/// <summary>
	/// Gets or sets the last updated at.
	/// </summary>
	/// <value>The last updated at.</value>
	public DateTimeOffset? LastUpdatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last updated by.
	/// </summary>
	/// <value>The last updated by.</value>
	public string LastUpdatedBy { get; set; }
}
