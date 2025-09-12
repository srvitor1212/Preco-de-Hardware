using Application.Jobs;
using Application.Jobs.Interfaces;
using Infra.Connect;
using Infra.Connect.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
    services.AdicionarSqlite();

    services.AdicionarServices();

    services.AddScoped<IScrapingJob, KabumScrapingJob>();

    services.AddHostedService<ScrapingWorker>();
})
.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddProvider(new CustomLoggerProvider());
})
.Build();

await host.RunAsync();
