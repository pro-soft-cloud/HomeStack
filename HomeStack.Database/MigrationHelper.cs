using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HomeStack.Core.Options;

namespace HomeStack.Database;

public static class MigrationHelper
{
	public static IHost MigrateDatabase<T>(this IHost host, ILogger logger) where T : DbContext
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(logger);

		logger.LogInformation("Migrating database...");

		using var scope = host.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<T>();

		LogDatabaseEngineVersion(db, logger);
		ApplyPendingMigrations(db, logger);

		return host;
	}

	private static void LogDatabaseEngineVersion(DbContext db, ILogger logger)
	{
		try
		{
			var conn = db.Database.GetDbConnection();
			var shouldClose = false;

			if (conn.State != ConnectionState.Open)
			{
				conn.Open();
				shouldClose = true;
			}

			try
			{
				var commandText = HomeStackOptions.DatabaseEngine switch
				{
					DatabaseEngine.MsSqlServer => "SELECT @@VERSION",
					DatabaseEngine.Postgres => "SELECT VERSION()",
					_ => null
				};

				string version;

				if (commandText is null)
				{
					version = string.Empty;
				}
				else
				{
					using (var cmd = conn.CreateCommand())
					{
						cmd.CommandText = commandText;
						version = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
					}
				}

				if (!string.IsNullOrWhiteSpace(version))
					logger.LogInformation("Connected to database engine: {Version}", version);
				else
					logger.LogWarning("Connected to undetected database engine");
			}
			finally
			{
				if (shouldClose)
					conn.Close();
			}
		}
		catch (Exception ex)
		{
			logger.LogWarning(ex, "Could not read database version");
		}
	}

	private static void ApplyPendingMigrations(DbContext db, ILogger logger)
	{
		logger.LogInformation("Checking for pending migrations.");

		var pendingMigrations = db.Database.GetPendingMigrations().ToList();

		if (pendingMigrations.Count == 0)
		{
			logger.LogInformation("No pending migrations found.");
			return;
		}

		var logMessage = new StringBuilder();

		logMessage.Append($"Found {pendingMigrations.Count} pending migrations:");

		foreach (var migration in pendingMigrations)
			logMessage.AppendLine().Append($"- {migration}");

		logger.LogInformation(logMessage.ToString());

		try
		{
			db.Database.Migrate();
			logger.LogInformation("Migrations successfully applied.");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An error occurred while migrating the database.");
			throw;
		}
	}
}