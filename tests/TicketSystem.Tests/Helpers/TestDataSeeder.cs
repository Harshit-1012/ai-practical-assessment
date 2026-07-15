using Microsoft.Extensions.DependencyInjection;
using TicketSystem.Api.Data;
using TicketSystem.Api.DTOs;
using TicketSystem.Api.Models;
using TicketSystem.Api.Models.Enums;

namespace TicketSystem.Tests.Helpers;

public static class TestDataSeeder
{
    public const int DefaultCreatedById = 3;
    public const int DefaultAssignedToId = 2;

    public static CreateTicketDto CreateValidTicketDto(
        string title = "Integration test ticket",
        string description = "Ticket created for integration testing",
        string priority = "Medium",
        int? assignedToId = DefaultAssignedToId,
        int createdById = DefaultCreatedById) =>
        new()
        {
            Title = title,
            Description = description,
            Priority = priority,
            AssignedToId = assignedToId,
            CreatedById = createdById
        };

    public static CreateCommentDto CreateValidCommentDto(
        string message = "Integration test comment",
        int createdById = DefaultCreatedById) =>
        new()
        {
            Message = message,
            CreatedById = createdById
        };

    public static async Task<int> SeedTicketAsync(
        IServiceProvider services,
        string status,
        string title = "Seeded ticket",
        string description = "Seeded for state machine tests")
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;

        var ticket = new Ticket
        {
            Title = title,
            Description = description,
            Priority = TicketPriority.Medium.ToString(),
            Status = status,
            AssignedToId = DefaultAssignedToId,
            CreatedById = DefaultCreatedById,
            CreatedAt = now,
            UpdatedAt = now
        };

        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();
        return ticket.Id;
    }

    public static async Task<string> GetTicketStatusAsync(IServiceProvider services, int ticketId)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ticket = await context.Tickets.FindAsync(ticketId);
        return ticket?.Status ?? throw new InvalidOperationException($"Ticket {ticketId} not found.");
    }
}
