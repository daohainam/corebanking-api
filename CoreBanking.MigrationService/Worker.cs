namespace CoreBanking.MigrationService;

public class Worker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        hostApplicationLifetime.StopApplication();

        return Task.CompletedTask;
    }
}
