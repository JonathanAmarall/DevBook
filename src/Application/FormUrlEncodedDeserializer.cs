using System.Reflection;
using System.Text.Json;
using Refit;

namespace Application;

public class FormUrlEncodedDeserializer : IHttpContentSerializer
{
    public async Task<T?> DeserializeAsync<T>(HttpContent content)
    {
        string data = await content.ReadAsStringAsync();

        var dict = data.Split('&')
            .Select(p => p.Split('='))
            .ToDictionary(k => k[0], v => Uri.UnescapeDataString(v[1]));

        string json = JsonSerializer.Serialize(dict);

        return JsonSerializer.Deserialize<T>(json);
    }

    public Task<HttpContent> SerializeAsync<T>(T item)
    {
        throw new NotImplementedException("Este serializer só é usado para desserialização.");
    }

    public HttpContent ToHttpContent<T>(T item)
    {
        throw new NotImplementedException();
    }

    public async Task<T?> FromHttpContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        string data = await content.ReadAsStringAsync(cancellationToken);

        var dict = data.Split('&')
            .Select(p => p.Split('='))
            .ToDictionary(k => k[0], v => Uri.UnescapeDataString(v[1]));

        string json = JsonSerializer.Serialize(dict);

        return JsonSerializer.Deserialize<T>(json);
    }

    public string? GetFieldNameForProperty(PropertyInfo propertyInfo)
    {
        throw new NotImplementedException();
    }

    public string? ContentType => "application/x-www-form-urlencoded";
}
