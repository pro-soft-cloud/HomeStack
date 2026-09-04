using HomeStack.Core.Default;
using HomeStack.Core.Exceptions;

namespace HomeStack.Core.Options;

public static class HomeStackOptions
{
	public static DatabaseEngine DatabaseEngine => DetermineDatabaseEngine();

	public static string ConnectionString => DetermineDatabaseConnectionString();

	private static string DetermineDatabaseConnectionString()
	{
		string databaseConnectionStringEnvName;

		switch (DatabaseEngine)
		{
			case DatabaseEngine.MsSqlServer:
				databaseConnectionStringEnvName = OptionVariable.DatabaseConnectionStringMsSqlServer;
				break;
			case DatabaseEngine.Postgres:
				databaseConnectionStringEnvName = OptionVariable.DatabaseConnectionStringPostgres;
				break;
			default:
				databaseConnectionStringEnvName = string.Empty;
				break;
		}

		var databaseConnectionString = Environment.GetEnvironmentVariable(databaseConnectionStringEnvName);

		return !string.IsNullOrWhiteSpace(databaseConnectionString) 
			? databaseConnectionString 
			: throw new HomeStackException($"Database connection string environment variable '{databaseConnectionStringEnvName}' not found or empty.");
	}

	private static DatabaseEngine DetermineDatabaseEngine()
	{
		const DatabaseEngine defaultEngine = DatabaseEngine.Postgres;

		try
		{
			var databaseEngineString = Environment.GetEnvironmentVariable(OptionVariable.DatabaseEngine);

			if (string.IsNullOrWhiteSpace(databaseEngineString))
			{
				return defaultEngine;
			}

			var engine = databaseEngineString.Trim();

			if (engine.Equals("mssqlserver", StringComparison.OrdinalIgnoreCase))
			{
				return DatabaseEngine.MsSqlServer;
			}

			if (engine.Equals("postgres", StringComparison.OrdinalIgnoreCase))
			{
				return DatabaseEngine.Postgres;
			}

			return defaultEngine;
		}
		catch (Exception)
		{
			return defaultEngine;
		}
	}
}
