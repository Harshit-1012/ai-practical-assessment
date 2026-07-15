using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TicketSystem.Api.DTOs;

namespace TicketSystem.Tests.Helpers;

public static class HttpClientJsonExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<(HttpStatusCode StatusCode, T? Body)> GetJsonAsync<T>(
        this HttpClient client,
        string requestUri)
    {
        var response = await client.GetAsync(requestUri);
        var body = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>(JsonOptions)
            : default;
        return (response.StatusCode, body);
    }

    public static async Task<(HttpStatusCode StatusCode, T? Body)> PostJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object payload)
    {
        var response = await client.PostAsJsonAsync(requestUri, payload, JsonOptions);
        var body = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>(JsonOptions)
            : default;
        return (response.StatusCode, body);
    }

    public static async Task<(HttpStatusCode StatusCode, T? Body)> PutJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        object payload)
    {
        var response = await client.PutAsJsonAsync(requestUri, payload, JsonOptions);
        var body = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>(JsonOptions)
            : default;
        return (response.StatusCode, body);
    }

    public static async Task<ApiError?> ReadApiErrorAsync(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<ApiError>(JsonOptions);
}
