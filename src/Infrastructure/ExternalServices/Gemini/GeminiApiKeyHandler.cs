using Microsoft.Extensions.Configuration;

namespace Infrastructure.ExternalServices.Gemini;

public class GeminiApiKeyHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;

    public GeminiApiKeyHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(request.RequestUri!);
        System.Collections.Specialized.NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["key"] = _configuration["GeminiApiSettings:ApiKey"];
        uriBuilder.Query = query.ToString();
        request.RequestUri = uriBuilder.Uri;

        return base.SendAsync(request, cancellationToken);
    }
}

