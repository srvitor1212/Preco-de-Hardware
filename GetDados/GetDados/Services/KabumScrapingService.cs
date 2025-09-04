using GetDados.DTO;
using HtmlAgilityPack;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GetDados.Services;

public class KabumScrapingService(HttpClient httpClient, string url) : WebScrapingService(httpClient, url)
{

    private static readonly string[] InvalidCategories = ["Hardware/Placa de vídeo (VGA)/Acessórios"];

    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public async Task<List<KabumDTO>> Executar()
    {
        var kabumDto = new List<KabumDTO>();

        while (true)
        {
            var result = await GetProducts();

            if (result.Count == 0)
                break;

            kabumDto.AddRange(result);

            NextPage("page_number");
        }

        return kabumDto;
    }

    private async Task<List<KabumDTO>> GetProducts()
    {
        var json = await GetJsonElement();

        json.TryGetProperty("data", out var dataProducts);

        // Salva um arquivo com o conteúdo até aqui
        string pretty = JsonSerializer.Serialize(dataProducts, options);
        await File.WriteAllTextAsync($"data_fixed_dataProducts_page_{Page}.json", pretty);

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

    private async Task<JsonElement> GetJsonElement()
    {

        // Faz a request que retorna um HTML
        var request = new HttpRequestMessage(HttpMethod.Get, Url);
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
        var jsonPretty = JsonSerializer.Serialize(data, options);
        string decoded = JsonSerializer.Deserialize<string>(jsonPretty)!;
        var jsonDoc = JsonDocument.Parse(decoded);
        // Salva um arquivo com o conteúdo até aqui
        string pretty = JsonSerializer.Serialize(jsonDoc.RootElement, options);
        await File.WriteAllTextAsync($"data_fixed_page_{Page}.json", pretty);


        // Navega no JSON até chegar na parte dos produtos
        var dataElements = jsonDoc.RootElement;
        dataElements.TryGetProperty("catalogServer", out var catalogServer);

        return catalogServer;
    }
}
