using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using TicketSystem.Blazor.Models;

namespace TicketSystem.Blazor.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorageService _tokenStorage;

    public CustomAuthStateProvider(ITokenStorageService tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _tokenStorage.GetUserAsync();
        var token = await _tokenStorage.GetTokenAsync();

        if (user is null || string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        ], "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
