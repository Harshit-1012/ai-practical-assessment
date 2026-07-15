-- Seed Data for TicketSystem Database
-- This data is automatically inserted by EF Core migrations
-- This file is for reference only

USE TicketSystemDb;
GO

-- Seed Users (automatically applied by migration)
-- These INSERT statements are already in the migration file

-- User 1: Admin
-- INSERT INTO Users (Id, Name, Email, Role) VALUES (1, 'Admin User', 'admin@ticketsystem.com', 'Admin');

-- User 2: Support Agent
-- INSERT INTO Users (Id, Name, Email, Role) VALUES (2, 'Support Agent', 'agent@ticketsystem.com', 'Agent');

-- User 3: Regular User
-- INSERT INTO Users (Id, Name, Email, Role) VALUES (3, 'Regular User', 'user@ticketsystem.com', 'User');

-- User 4: Jane Smith (Agent)
-- INSERT INTO Users (Id, Name, Email, Role) VALUES (4, 'Jane Smith', 'jane.smith@ticketsystem.com', 'Agent');

-- User 5: Bob Johnson (User)
-- INSERT INTO Users (Id, Name, Email, Role) VALUES (5, 'Bob Johnson', 'bob.johnson@ticketsystem.com', 'User');

GO

-- Note: The actual seed data is in the EF Core migration file:
-- src/TicketSystem.Api/Migrations/20260714093803_InitialCreate.cs
-- This script is for reference to understand what data exists in the database.
