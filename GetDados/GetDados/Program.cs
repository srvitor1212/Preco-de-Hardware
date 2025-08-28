using GetDados.Services;

HttpClient httpClient = new HttpClient();

var motor = new WebScrapingService(
    httpClient, 
    "https://www.kabum.com.br/hardware/placa-de-video-vga?page_number=1&page_size=100&facet_filters=&sort=price");

var produtosKabum = await motor.Kabum();

foreach(var item in produtosKabum)
    Console.WriteLine($"{item.Name} | {item.Price}");