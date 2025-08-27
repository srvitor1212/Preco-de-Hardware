using GetDados.DTO;
using HtmlAgilityPack;
using System.Text.Json;

namespace GetDados.Services;

public class WebScrapingService(HttpClient httpClient)
{
    public HttpClient HttpClient { get; } = httpClient;

    public async Task<List<KabumDTO>> Kabum()
    {
        var URL = "https://www.kabum.com.br/hardware/placa-de-video-vga?page_number=1&page_size=20&facet_filters=eyJwcmljZSI6eyJtaW4iOjUwMCwibWF4IjoxMTc2NDcuMDV9fQ==&sort=price";


        // Faz a request que retorna um HTML
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


        // Navega no JSON até chegar na parte string
        using var docJson = JsonDocument.Parse(json);
        var root = docJson.RootElement;
        root.TryGetProperty("props", out var props);
        props.TryGetProperty("pageProps", out var pageProps);
        pageProps.TryGetProperty("data", out var data);


        // Transforma o string em um JsonDocument
        var options = new JsonSerializerOptions { WriteIndented = true };
        var jsonPretty = JsonSerializer.Serialize(data, options);
        string decoded = JsonSerializer.Deserialize<string>(jsonPretty)!;
        var jsonDoc = JsonDocument.Parse(decoded);
        // Salva um arquivo com o conteúdo até aqui
        string pretty = JsonSerializer.Serialize(jsonDoc.RootElement, options);
        await File.WriteAllTextAsync("data_fixed.json", pretty);


        // Navega no JSON até chegar na parte dos produtos
        var dataElements = jsonDoc.RootElement;
        dataElements.TryGetProperty("catalogServer", out var catalogServer);
        catalogServer.TryGetProperty("data", out var dataProducts);
        // Salva um arquivo com o conteúdo até aqui
        pretty = JsonSerializer.Serialize(dataProducts, options);
        await File.WriteAllTextAsync("data_fixed_dataProducts.json", pretty);


        // Transforma em um objeto para retorno
        var productList = new List<KabumDTO>();
        foreach (var item in dataProducts.EnumerateArray())
        {
            productList.Add(new KabumDTO
            {
                Name = item.GetProperty("name").GetString() ?? string.Empty,
                Price = item.GetProperty("price").GetDecimal(),
                FriendlyName = item.GetProperty("friendlyName").GetString() ?? string.Empty
            });
        }

        return productList;
    }
}
