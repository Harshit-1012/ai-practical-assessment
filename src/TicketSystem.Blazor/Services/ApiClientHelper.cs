using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class ApiClientHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public ApiClientHelper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    public async Task<T> PostAsync<T>(string uri, object body, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    public async Task<T> PutAsync<T>(string uri, object body, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(uri, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    public async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        ApiErrorResponse? error = null;

        try
        {
            error = JsonSerializer.Deserialize<ApiErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            // Use raw content below.
        }

        var message = error?.Message
            ?? error?.Error
            ?? error?.Detail
            ?? (error?.Errors is not null
                ? string.Join("; ", error.Errors.SelectMany(e => e.Value))
                : content);

        throw new ApiClientException(message, response.StatusCode, error);
    }
}

public class ApiClientException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ApiErrorResponse? ErrorResponse { get; }

    public ApiClientException(string message, HttpStatusCode statusCode, ApiErrorResponse? errorResponse = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorResponse = errorResponse;
    }
}
