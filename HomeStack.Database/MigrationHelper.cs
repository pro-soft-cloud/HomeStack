using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProSoft.HomeStack.Core.Options;

namespace ProSoft.HomeStack.Database;

public static class MigrationHelper
{
	public static IHost MigrateDatabase<T>(this IHost host, ILogger logger) where T : DbContext
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(logger);

		logger.LogInformation("Migrating database...");

		using (var scope = host.Services.CreateScope())
		{
			var services = scope.ServiceProvider;

			try
			{
				var db = services.GetRequiredService<T>();

				try
				{
					var conn = db.Database.GetDbConnection();
					var shouldClose = false;

					if (conn.State != System.Data.ConnectionState.Open)
					{
						conn.Open();
						shouldClose = true;
					}

					var version = string.Empty;
					if (string.IsNullOrWhiteSpace(version))
					{
						using (var cmd = conn.CreateCommand())
						{
							switch (HomeStackOptions.DatabaseEngine)
							{
								case DatabaseEngine.MsSqlServer:
									cmd.CommandText = "SELECT @@VERSION";
									break;

								case DatabaseEngine.Postgres:
									cmd.CommandText = "SELECT VERSION()";
									break;
							}

							version = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
						}
					}

					if (!string.IsNullOrWhiteSpace(version))
						logger.LogInformation("Connected to database engine: {Version}", version);
					else
						logger.LogWarning("Connected to undetected database engine");
				}
				catch (Exception ex)
				{
					logger.LogWarning(ex, "Could not read database version");
				}

				logger.LogInformation("Checking for pending migrations.");

				var pendingMigrations = db.Database.GetPendingMigrations().ToList();

				if (pendingMigrations.Any())
				{
					var logMessage = new StringBuilder();
					logMessage.Append($"Found {pendingMigrations.Count} pending migrations:");

					foreach (var migration in pendingMigrations)
						logMessage.AppendLine().Append($"- {migration}");

					logger.LogInformation(logMessage.ToString());
					db.Database.Migrate();
					logger.LogInformation("Migrations successfully applied.");
				}
				else
				{
					logger.LogInformation("No pending migrations found.");
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while migrating the database.");
			}
		}

		return host;
	}
}
