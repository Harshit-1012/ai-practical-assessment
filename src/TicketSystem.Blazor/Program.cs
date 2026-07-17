using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using TicketSystem.Blazor;
using TicketSystem.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5041";
if (!apiBaseUrl.EndsWith('/'))
{
    apiBaseUrl += "/";
}

builder.Services.AddScoped(_ =>
{
    var client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    client.Timeout = TimeSpan.FromSeconds(30);
    return client;
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiClientHelper>();
builder.Services.AddScoped<ITicketApiService, TicketApiService>();
builder.Services.AddScoped<ICommentApiService, CommentApiService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<ITicketWorkflowService, TicketWorkflowService>();
builder.Services.AddScoped<ITicketDisplayService, TicketDisplayService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var host = builder.Build();

var authService = host.Services.GetRequiredService<IAuthService>();
await authService.InitializeAsync();

await host.RunAsync();
