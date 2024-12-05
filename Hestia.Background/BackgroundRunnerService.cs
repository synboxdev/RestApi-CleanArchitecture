using Hestia.Background.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hestia.Background;

public class BackgroundRunnerService(IServiceScopeFactory scopeFactory, ILogger<BackgroundRunnerService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var taskList = CreateTaskList(stoppingToken);
        return Task.WhenAll(taskList);
    }

    private List<Task> CreateTaskList(CancellationToken stoppingToken)
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var scheduledTasks = scope.ServiceProvider.GetRequiredService<IEnumerable<IScheduledTask>>().ToList();
            return scheduledTasks.Select(task => ExecuteTaskAsync(task, stoppingToken)).ToList();
        }
    }

    private async Task ExecuteTaskAsync(IScheduledTask scheduledTask, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                try
                {
                    if (scope.ServiceProvider.GetService(scheduledTask.GetType()) is IScheduledTask scopedTask)
                    {
                        await scopedTask.ExecuteAsync(stoppingToken);
                        logger.LogInformation($"{scheduledTask.GetType().Name} executed successfully.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred executing {scheduledTask.GetType().Name}.");
                }
            }

            // Wait for the specific interval before executing the next iteration
            await Task.Delay(scheduledTask.Interval, stoppingToken);
        }
    }
}