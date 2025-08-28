namespace GetDados.Services;

public abstract class DefaultService(
    HttpClient httpClient,
    string Url)
{
    protected readonly HttpClient _httpClient = httpClient;

    protected string _url = Url;
}
