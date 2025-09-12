using System.Collections.Specialized;
using System.Web;

namespace Application.Services;

public abstract class WebScrapingService
{
    protected HttpClient HttpClient { get; }
    public Uri CurrentUri { get; private set; }
    public NameValueCollection QueryParams { get; private set; }
    public NameValueCollection InitialQueryParams { get; private set; }
    public int Page { get; private set; } = 1;

    protected WebScrapingService(HttpClient httpClient, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL não pode ser nula ou vazia.", nameof(baseUrl));

        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        CurrentUri = new Uri(baseUrl);

        HttpClient.BaseAddress = new Uri($"{CurrentUri.Scheme}://{CurrentUri.Host}");

        QueryParams = HttpUtility.ParseQueryString(CurrentUri.Query);
        InitialQueryParams = HttpUtility.ParseQueryString(CurrentUri.Query);
    }

    protected void NextPage(string pageParamName)
    {
        Page++;

        QueryParams[pageParamName] = Page.ToString();

        var uriBuilder = new UriBuilder(CurrentUri)
        {
            Query = QueryParams.ToString()!
        };

        CurrentUri = uriBuilder.Uri;
    }

    protected async Task<HttpResponseMessage> GetContentAsync()
    {
        var response = await HttpClient.GetAsync(CurrentUri.PathAndQuery);
        response.EnsureSuccessStatusCode();
        return response;
    }

    protected void ResetPagination()
    {
        Page = 1;
        QueryParams = new NameValueCollection(InitialQueryParams);
        CurrentUri = new UriBuilder(CurrentUri) { Query = QueryParams.ToString()! }.Uri;
    }
}
