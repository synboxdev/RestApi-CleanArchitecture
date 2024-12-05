namespace Hestia.Background.Interfaces;

public interface IScheduledTask
{
    TimeSpan Interval { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}