# Candidate Information

## Personal Details

**Name:** Harshit Gupta
**Role:** Senior Software Engineer 
**Assessment Start Date:** 13-07-2026  
**Submission Date:** 17-07-2026

## Project Selection

**Project Option Selected:** Backend-Heavy - Support Ticket Management System  
**Primary AI Tool Used:** Cursor AI  
**Primary Technology Stack:** .NET 9, Blazor WebAssembly, ASP.NET Core Web API, SQL Server, Entity Framework Core

## Project Summary

Built a full-stack Support Ticket Management System with a Blazor WebAssembly frontend and ASP.NET Core Web API backend on .NET 9, backed by SQL Server LocalDB via EF Core. The signature piece is a server-enforced ticket state machine (valid transitions only), plus comments, search/filter, and 28 integration tests. Stretch work added minimal JWT authentication (demo user sign-in, protected mutation endpoints, `CreatedById` from claims).

## Tools Used

### Development Tools
- Visual Studio 2022
- Cursor AI
- Git

### Technologies
- .NET 9 SDK
- Blazor WebAssembly
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- xUnit

### AI Tools
- Cursor AI (primary tool for code generation, debugging, code review)
- 

## Setup Summary

Started from the assessment template and organized the solution into three projects: `TicketSystem.Api` (Web API), `TicketSystem.Blazor` (WASM UI), and `TicketSystem.Tests` (xUnit integration tests). Local development uses SQL Server LocalDB with EF Core migrations; the API and Blazor apps run on separate `http` launch profiles (ports 5041 and 5036). Cursor AI was the primary assistant, guided by `.cursorrules` and `tool-specific/cursor-workflow/` context files for conventions, state machine rules, and lifecycle documentation.
