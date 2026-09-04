namespace HomeStack.Core.Exceptions;

[Serializable]
public class HomeStackException : Exception
{
	public HomeStackException()
    {
    }

    public HomeStackException(string message)
        : base($"HomeStack Error: {message}")
    {

    }

    public HomeStackException(string message, Exception innerException)
        : base($"HomeStack Error: {message}", innerException)
    {

    }
}