using Hestia.Background.Interfaces;
using Hestia.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hestia.Background.Tasks;

public class TokenCleanupTask(IServiceProvider serviceProvider, ILogger<TokenCleanupTask> logger) : IScheduledTask
{
    public TimeSpan Interval => TimeSpan.FromHours(1);

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (var scope = serviceProvider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<HestiaContext>();

            var expiredTokens = await context.TokenLog
                .Where(t => t.TokenExpirationDate < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (expiredTokens.Count != 0)
            {
                context.TokenLog.RemoveRange(expiredTokens);
                int result = await context.SaveChangesAsync(cancellationToken);

                if (result > 0)
                    logger.LogInformation($"Total of [{result}] expired tokens from TokenLog table have been removed.");
            }
        }
    }
}