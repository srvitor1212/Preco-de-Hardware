using GetDados.Services;

HttpClient httpClient = new HttpClient();

var motor = new WebScrapingService(httpClient);

var produtosKabum = await motor.Kabum();

foreach(var item in produtosKabum)
    Console.WriteLine($"{item.Name} {item.FriendlyName} {item.Price}");