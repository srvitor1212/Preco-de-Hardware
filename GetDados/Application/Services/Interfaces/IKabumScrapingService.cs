namespace Application.Services.Interfaces;

public interface IKabumScrapingService
{
    Task ExecutarAsync(CancellationToken cancellationToken);
}
