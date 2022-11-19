using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QuestPDF.Previewer;

internal static class Helpers
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(this HttpClient client, string? requestUri, TValue value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        return client.PostAsync(requestUri, jsonContent, cancellationToken);
    }
}