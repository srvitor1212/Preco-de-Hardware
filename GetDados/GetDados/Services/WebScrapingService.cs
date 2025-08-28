using GetDados.DTO;
using HtmlAgilityPack;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GetDados.Services;

public class WebScrapingService(
    HttpClient httpClient,
    string Url) : DefaultService(httpClient, Url)
{
    private static readonly string[] InvalidCategories = ["Hardware/Placa de vídeo (VGA)/Acessórios"];

    public async Task<List<KabumDTO>> Kabum()
    {

        // Faz a request que retorna um HTML
        var request = new HttpRequestMessage(HttpMethod.Get, _url);
        request.Headers.Add("Cookie", "isMobile=false; isMobileDevice=false; storeCode=001");
        var response = await _httpClient.SendAsync(request);
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
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
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
            var category = item.GetProperty("category").GetString() ?? string.Empty;

            if (InvalidCategories.Contains(category))
                continue;

            item.TryGetProperty("manufacturer", out var manufacturer);

            productList.Add(new KabumDTO
            {
                Name = item.GetProperty("name").GetString() ?? string.Empty,
                FriendlyName = item.GetProperty("friendlyName").GetString() ?? string.Empty,
                Description = item.GetProperty("description").GetString() ?? string.Empty,
                Category = category,
                ManufacturerName = manufacturer.GetProperty("name").GetString() ?? string.Empty,
                Price = item.GetProperty("price").GetDecimal(),
                PrimePrice = item.GetProperty("primePrice").GetDecimal(),
                PrimePriceWithDiscount = item.GetProperty("primePriceWithDiscount").GetDecimal(),
                OldPrice = item.GetProperty("oldPrice").GetDecimal(),
                OldPrimePrice = item.GetProperty("oldPrimePrice").GetDecimal(),
                PriceWithDiscount = item.GetProperty("priceWithDiscount").GetDecimal(),
                PriceMarketplace = item.GetProperty("priceMarketplace").GetDecimal(),
                Available = item.GetProperty("available").GetBoolean(),
            });
        }

        return productList;
    }
}
