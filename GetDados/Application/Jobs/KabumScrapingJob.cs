using Application.Jobs.Interfaces;
using Application.Services.Interfaces;

namespace Application.Jobs;

public class KabumScrapingJob(IKabumScrapingService service) : IScrapingJob
{
    private readonly IKabumScrapingService _service = service;

    public string Name => "Kabum";

    public string CronExpression => "0 0 */12 * * *";

    public async Task ExecutarAsync(CancellationToken stoppingToken)
    {
        await _service.ExecutarAsync(stoppingToken);
    }
}
