namespace ProSoft.HomeStack.Core.Default;

/// <summary>
/// Defaults for dates.
/// </summary>
public static class Date
{
	/// <summary>
	/// The default minimum date.
	/// </summary>
	public static readonly DateTimeOffset MinDate = new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeSpan.Zero);

	/// <summary>
	/// The default maximum date.
	/// </summary>
	public static readonly DateTimeOffset MaxDate = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);
}
