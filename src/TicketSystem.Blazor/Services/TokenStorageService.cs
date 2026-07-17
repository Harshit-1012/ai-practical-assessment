using System.Text.Json;
using Microsoft.JSInterop;
using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public interface ITokenStorageService
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task<AuthenticatedUser?> GetUserAsync();
    Task SetUserAsync(AuthenticatedUser user);
    Task ClearAsync();
}

public class TokenStorageService : ITokenStorageService
{
    private const string TokenKey = "auth_token";
    private const string UserKey = "auth_user";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _jsRuntime;

    public TokenStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync() =>
        await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);

    public async Task SetTokenAsync(string token) =>
        await _jsRuntime.InvokeAsync<object?>("localStorage.setItem", TokenKey, token);

    public async Task<AuthenticatedUser?> GetUserAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", UserKey);
        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<AuthenticatedUser>(json, JsonOptions);
    }

    public async Task SetUserAsync(AuthenticatedUser user)
    {
        var json = JsonSerializer.Serialize(user, JsonOptions);
        await _jsRuntime.InvokeAsync<object?>("localStorage.setItem", UserKey, json);
    }

    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeAsync<object?>("localStorage.removeItem", TokenKey);
        await _jsRuntime.InvokeAsync<object?>("localStorage.removeItem", UserKey);
    }
}
