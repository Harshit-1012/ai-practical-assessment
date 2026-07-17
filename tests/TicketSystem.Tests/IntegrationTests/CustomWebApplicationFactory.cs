using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.Api.Data;
using TicketSystem.Api.Models;

namespace TicketSystem.Tests.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection? _connection;

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Comments.RemoveRange(context.Comments);
        context.Tickets.RemoveRange(context.Tickets);
        await context.SaveChangesAsync();
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
        await EnsureUsersSeededAsync(context);
    }

    private static async Task EnsureUsersSeededAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        context.Users.AddRange(
            new User { Id = 1, Name = "Admin User", Email = "admin@ticketsystem.com", Role = "Admin" },
            new User { Id = 2, Name = "Support Agent", Email = "agent@ticketsystem.com", Role = "Agent" },
            new User { Id = 3, Name = "Regular User", Email = "user@ticketsystem.com", Role = "User" },
            new User { Id = 4, Name = "Jane Smith", Email = "jane.smith@ticketsystem.com", Role = "Agent" },
            new User { Id = 5, Name = "Bob Johnson", Email = "bob.johnson@ticketsystem.com", Role = "User" });

        await context.SaveChangesAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs skips SQL Server registration when environment is "Testing".
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "TicketSystemDevSecretKey_Min32Chars!!",
                ["Jwt:Issuer"] = "TicketSystem.Api",
                ["Jwt:Audience"] = "TicketSystem.Blazor",
                ["Jwt:ExpiryMinutes"] = "480"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            _connection ??= new SqliteConnection("DataSource=:memory:");
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }
}
