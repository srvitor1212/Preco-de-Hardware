using Application.Jobs.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Jobs;

public class ScrapingWorker(
    IEnumerable<IScrapingJob> jobs,
    ILogger<ScrapingWorker> logger) : BackgroundService
{
    private readonly IEnumerable<IScrapingJob> _jobs = jobs;
    private readonly ILogger<ScrapingWorker> _logger = logger;
    private Dictionary<IScrapingJob, DateTimeOffset> _nextJob = null!;

    private static DateTimeOffset Now => DateTimeOffset.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ScrapingWorker foi iniciado.");

        _nextJob = _jobs.ToDictionary(
            job => job,
            job => Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                RunJobs(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao disparar jobs - {ExMessage}", ex.Message);
            }
        }

        _logger.LogInformation("ScrapingWorker foi finalizado.");
    }

    private void RunJobs(CancellationToken stoppingToken)
    {
        foreach (var job in _jobs)
        {
            if (_nextJob[job] <= Now)
                RunThisJob(job, stoppingToken);
        }
    }

    private void RunThisJob(IScrapingJob job, CancellationToken stoppingToken)
    {
        _ = Task.Run(async () =>
        {
            Thread.CurrentThread.Name = job.Name;
            try
            {
                _logger.LogInformation("Executando job {Nome}", job.Name);
                await job.ExecutarAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no job {Nome}", job.Name);
            }
        }, stoppingToken);

        _nextJob[job] = GetNextOccurrence(job.CronExpression, Now);
    }

    private static DateTimeOffset GetNextOccurrence(string cronExpression, DateTimeOffset fromUtc)
    {
        var cron = Cronos.CronExpression.Parse(cronExpression, Cronos.CronFormat.IncludeSeconds);
        return cron.GetNextOccurrence(fromUtc, TimeZoneInfo.Utc) ?? fromUtc.AddYears(100);
    }

}
