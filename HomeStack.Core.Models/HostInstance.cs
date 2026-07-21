using System.Net;

namespace HomeStack.Core.Models;

public sealed class HostInstance : BaseObject
{
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	/// <value>The identifier.</value>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the display name.
	/// </summary>
	/// <value>The display name.</value>
	public string DisplayName { get; set; } = "new hostinstance";

	/// <summary>
	/// Gets or sets the ip.
	/// </summary>
	/// <value>The ip.</value>
	public IPAddress IP { get; set; }
}
