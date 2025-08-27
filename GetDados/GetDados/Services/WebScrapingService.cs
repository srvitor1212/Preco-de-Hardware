using HtmlAgilityPack;
using System.Text.Json;

namespace GetDados.Services;

public class WebScrapingService(HttpClient httpClient)
{
    public HttpClient HttpClient { get; } = httpClient;

    public async Task Kabum()
    {
        var URL = "https://www.kabum.com.br/hardware/placa-de-video-vga?page_number=1&page_size=20&facet_filters=eyJwcmljZSI6eyJtaW4iOjUwMCwibWF4IjoxMTc2NDcuMDV9fQ==&sort=price";

        var request = new HttpRequestMessage(HttpMethod.Get, URL);
        request.Headers.Add("Cookie", "isMobile=false; isMobileDevice=false; storeCode=001");
        var response = await HttpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();




        // carregar HTML
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var scriptNode = doc.DocumentNode
            .SelectSingleNode("//script[@id='__NEXT_DATA__']");
        if (scriptNode == null)
            throw new Exception("Script __NEXT_DATA__ não encontrado.");
        var json = scriptNode.InnerText;


        // analisar JSON
        using var docJson = JsonDocument.Parse(json);
        var root = docJson.RootElement;
        root.TryGetProperty("props", out var props);
        props.TryGetProperty("pageProps", out var pageProps);
        pageProps.TryGetProperty("data", out var data);





        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonPretty = JsonSerializer.Serialize(data, options);

        Console.WriteLine(Directory.GetCurrentDirectory());
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
        await File.WriteAllTextAsync(filePath, jsonPretty);






        // lê o conteúdo bruto do arquivo
        var raw = await File.ReadAllTextAsync("data.json");

        // 1º passo: desserializar para string (isso decodifica os \u0022 em aspas)
        string decoded = JsonSerializer.Deserialize<string>(raw)!;

        // 2º passo (opcional): reformatar/identar para salvar como JSON bonito
        var jsonDoc = JsonDocument.Parse(decoded);
        string pretty = JsonSerializer.Serialize(jsonDoc.RootElement, options);

        // salva no arquivo final
        await File.WriteAllTextAsync("data_fixed.json", pretty);


    }
}
