using Microsoft.EntityFrameworkCore;
using TicketSystem.Api.Models;

namespace TicketSystem.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);
        });

        // Ticket Configuration
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(5000);

            entity.Property(t => t.Priority)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.CreatedAt)
                .IsRequired();

            entity.Property(t => t.UpdatedAt)
                .IsRequired();

            // Relationship: Ticket -> AssignedTo (User) - Optional
            entity.HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship: Ticket -> CreatedBy (User) - Required
            entity.HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Ticket -> Comments
            entity.HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Comment Configuration
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(c => c.CreatedAt)
                .IsRequired();

            // Relationship: Comment -> CreatedBy (User) - Required
            entity.HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Data - Sample Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@ticketsystem.com",
                Role = "Admin"
            },
            new User
            {
                Id = 2,
                Name = "Support Agent",
                Email = "agent@ticketsystem.com",
                Role = "Agent"
            },
            new User
            {
                Id = 3,
                Name = "Regular User",
                Email = "user@ticketsystem.com",
                Role = "User"
            },
            new User
            {
                Id = 4,
                Name = "Jane Smith",
                Email = "jane.smith@ticketsystem.com",
                Role = "Agent"
            },
            new User
            {
                Id = 5,
                Name = "Bob Johnson",
                Email = "bob.johnson@ticketsystem.com",
                Role = "User"
            }
        );
    }
}
