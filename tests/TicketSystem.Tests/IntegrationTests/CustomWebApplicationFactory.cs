using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.Api.Data;

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
