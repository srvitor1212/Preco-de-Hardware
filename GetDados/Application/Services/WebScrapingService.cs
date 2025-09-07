using System.Collections.Specialized;
using System.Web;

namespace Application.Services;

public abstract class WebScrapingService
{
    public HttpClient HttpClient { get; private set; }
    public string Url { get; private set; }
    public Uri Uri { get; private set; }
    public NameValueCollection QueryParams { get; private set; }
    public NameValueCollection InitialQueryParams { get; private set; }
    public int Page { get; set; } = 1;

    protected WebScrapingService(HttpClient httpClient, string url)
    {
        HttpClient = httpClient;
        Url = url;
        Uri = new Uri(Url);

        HttpClient.BaseAddress = Uri;
        QueryParams = HttpUtility.ParseQueryString(Uri!.Query);
        InitialQueryParams = HttpUtility.ParseQueryString(Uri!.Query);
    }

    protected void NextPage(string pageElementName)
    {
        Page++;

        QueryParams[pageElementName] = Page.ToString();

        var uriBuilder = new UriBuilder(Uri)
        {
            Query = QueryParams.ToString()!
        };

        Uri = uriBuilder.Uri;
        Url = Uri.ToString();
    }
}
