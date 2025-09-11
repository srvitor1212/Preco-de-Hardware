using Application.Jobs;
using Application.Jobs.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using Infra.Connect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
    services.AdicionarSqlite();

    services.AddHttpClient<IKabumScrapingService, KabumScrapingService>();
    services.AddScoped<IScrapingJob, KabumScrapingJob>();

    services.AddHostedService<ScrapingWorker>();
})
.Build();

await host.RunAsync();
