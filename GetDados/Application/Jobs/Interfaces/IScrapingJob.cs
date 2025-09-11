namespace Application.Jobs.Interfaces;

public interface IScrapingJob
{
    string Name { get; }
    string CronExpression { get; };
    Task ExecutarAsync(CancellationToken stoppingToken);
}
