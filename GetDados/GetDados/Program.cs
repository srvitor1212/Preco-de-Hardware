using GetDados.Services;

HttpClient httpClient = new HttpClient();

var motor = new WebScrapingService(httpClient);

await motor.Kabum();