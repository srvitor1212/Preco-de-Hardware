using GetDados.Services;

Console.WriteLine($"Dir={Environment.CurrentDirectory}");

HttpClient httpClient = new();

var motor = new KabumScrapingService(
    httpClient, 
    "https://www.kabum.com.br/hardware/placa-de-video-vga?page_number=1&page_size=100&facet_filters=&sort=price");

var produtosKabum = await motor.Executar();

foreach(var item in produtosKabum)
    Console.WriteLine($"{item.Name} | {item.Price}");