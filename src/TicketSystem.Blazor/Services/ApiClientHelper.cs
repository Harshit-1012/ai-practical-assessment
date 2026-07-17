using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class ApiClientHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;

    public ApiClientHelper(HttpClient httpClient, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
    {
        await AttachAuthHeaderAsync();
        var response = await _httpClient.GetAsync(uri, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    public async Task<T> PostAsync<T>(string uri, object body, CancellationToken cancellationToken = default)
    {
        await AttachAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync(uri, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    public async Task<T> PutAsync<T>(string uri, object body, CancellationToken cancellationToken = default)
    {
        await AttachAuthHeaderAsync();
        var response = await _httpClient.PutAsJsonAsync(uri, body, cancellationToken);
        await EnsureSuccessAsync(response);
        return (await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken))!;
    }

    private async Task AttachAuthHeaderAsync()
    {
        var token = await _tokenStorage.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
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

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            message = string.IsNullOrWhiteSpace(message)
                ? "You must sign in to perform this action."
                : message;
        }

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
