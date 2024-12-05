using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

namespace Hestia.Persistence.Contexts.Extensions;

public static class DbContextExtensions
{
    public static void EnsureMigrationsApplied(this DbContext context, ILogger logger)
    {
        try
        {
            var applied = context.GetService<IHistoryRepository>().GetAppliedMigrations().Select(m => m.MigrationId);
            var total = context.GetService<IMigrationsAssembly>().Migrations.Select(m => m.Key);

            if (total.Except(applied).Any())
            {
                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No new migrations to apply.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations.");
            throw;
        }
    }
}