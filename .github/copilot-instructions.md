# Avalans Project - AI Coding Agent Instructions

## Agent Interaction Requirements (MANDATORY)

Before making any code changes, an AI agent (Copilot) MUST do the following, in this exact order:

1. Present a concise **List of Assumptions** the agent is making about the repository, environment, or user intent.
2. Present a concise **Planned Tasks** list (step-by-step) describing what edits the agent will perform.
3. Request explicit permission to proceed. The agent must wait for an affirmative confirmation from a human before applying any changes.

4. Consult the project's design guidelines and ADRs via the Hexmaster MCP server and record a short compliance summary in the Planned Tasks (see below).

	- Use the MCP tools `mcp_hexmaster-des_list_docs`, `mcp_hexmaster-des_get_doc`, or `mcp_hexmaster-des_search_docs` to fetch applicable design guidelines, ADRs, and rules before editing.
	- Include a one-line **Guideline Compliance** item in the Planned Tasks summarizing any constraints or required patterns (for example: "Guideline Compliance: follow ADR-001 naming and logging conventions; use OpenTelemetry exporters configured by ServiceDefaults").
	- If no relevant ADRs apply, state that explicitly: "Guideline Compliance: no applicable ADRs found."

Use the `manage_todo_list` tool to create and publish the planned tasks before edits. Example template the agent should use when asking for permission:

Assumptions:
- Running the project uses Aspire AppHost (`src/Aspire/.../AppHost`).
- All projects target `net10.0` and use implicit usings/nullable.

Planned Tasks:
1. Update `X` to `Y` in `file.cs`.
2. Run `dotnet build` for the solution.

Request: Please confirm you want me to proceed with the tasks above (reply `yes` to continue).

Agents that fail to present Assumptions, Planned Tasks, or to explicitly request permission must not change files.

## Architecture Overview

This is a **microservices-based .NET solution** using **.NET Aspire** orchestration. The solution follows a vertical slice architecture with three domain-bounded services:

- **Items** - Inventory/item management (Azure Table Storage)
- **Locations** - Location management with external integrations (Azure Table Storage)
- **Transactions** - Transaction processing (MongoDB)

Each service follows the same layered structure:
- `*.Api` - Minimal API endpoints with Aspire ServiceDefaults
- `*.Abstractions` - Interfaces and contracts
- Core domain logic (e.g., `Bexter.Avalans.Items`)
- `*.Data.*` - Data access layer (TableStorage or MongoDb implementations)
- `*.Integrations` - External service integrations (Locations only)

## Technology Stack

- **.NET 10.0** (all projects target `net10.0`)
- **.NET Aspire 9.5** for orchestration, service discovery, observability
- **Azure Table Storage** for Items and Locations
- **MongoDB** for Transactions
- **OpenTelemetry** for distributed tracing and metrics
- **Minimal APIs** with OpenAPI/Swagger support

## Project Structure & Naming Conventions

All projects follow the pattern: `Bexter.Avalans.<Domain>.<Layer>`

Solution folders mirror directory structure:
- `src/Aspire/` - AppHost orchestrator and shared ServiceDefaults
- `src/Items/`, `src/Locations/`, `src/Transactions/` - Domain services
- `src/Shared/` - Cross-cutting concerns (`Bexter.Avalans.Core`)

## Development Workflow

### Running the Application

**Always use Aspire AppHost** to run the entire solution:
```powershell
cd src/Aspire/Bexter.Avalans.Aspire/Bexter.Avalans.Aspire.AppHost
dotnet run
```

This launches:
- Aspire Dashboard (https://localhost:17166 or http://localhost:15081)
- All three API services with service discovery
- Distributed tracing and health checks

**Do not run individual API projects directly** - they are orchestrated by Aspire.

### Building

Build from solution root:
```powershell
cd src
dotnet build Avalans.sln
```

### HTTP Testing

Each API has a `.http` file for testing endpoints:
- `Items.Api.http` - http://localhost:5202
- `Locations.Api.http` - (check launchSettings.json for port)
- `Transactions.Api.http` - (check launchSettings.json for port)

Use VS Code REST Client extension or similar.

## Key Patterns & Conventions

### Service Defaults Pattern

All API projects reference `Bexter.Avalans.Aspire.ServiceDefaults` and call:
```csharp
builder.AddServiceDefaults(); // Adds OpenTelemetry, health checks, service discovery
app.MapDefaultEndpoints();     // Maps /health and /alive endpoints
```

This centralizes cross-cutting concerns. **Never duplicate this configuration** in individual APIs.

### Project References

- APIs reference only `ServiceDefaults` and their own domain layer
- Aspire AppHost references all three API projects
- Data layers are implementation details, not referenced by APIs directly (yet)

### Nullable & Implicit Usings

All projects enable:
```xml
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

**Always use nullable reference types** and leverage implicit usings for common namespaces.

### Health Checks

Health endpoints (`/health`, `/alive`) are **only exposed in Development** for security. See `Extensions.cs` in `src/Aspire/.../Bexter.Avalans.Aspire.ServiceDefaults` for implementation.

### Data Storage Strategy

- **Items & Locations**: Azure Table Storage (NoSQL key-value)
- **Transactions**: MongoDB (document store)

Choose storage based on domain requirements, not uniformity.

## Project Dependencies

Key NuGet packages:
- `Aspire.Hosting.AppHost` 9.5.0 (AppHost only)
- `Microsoft.Extensions.Http.Resilience` 9.9.0 (ServiceDefaults)
- `Microsoft.Extensions.ServiceDiscovery` 9.5.0 (ServiceDefaults)
- `OpenTelemetry.*` packages for observability

## What This Project Is NOT (Yet)

This is currently a **greenfield scaffold**:
- API endpoints are placeholder WeatherForecast examples
- No real domain logic implemented in core libraries (empty projects)
- No database connections configured in appsettings
- No authentication/authorization
- No CI/CD pipelines

## When Adding New Features

1. **Identify the domain** - Items, Locations, or Transactions?
2. **Add domain logic** to `Bexter.Avalans.<Domain>` project
3. **Define interfaces** in `*.Abstractions` project
4. **Implement data access** in `*.Data.*` project
5. **Add API endpoints** in `*.Api/Program.cs` using Minimal APIs
6. **Update AppHost** if new services are added (rare)
7. **Test via .http files** and Aspire Dashboard

## Common Pitfalls

- ❌ Don't run APIs independently - use AppHost
- ❌ Don't add ServiceDefaults configuration to individual APIs
- ❌ Don't share data access implementations across domains
- ❌ Don't add dependencies between domain services (Items ↔ Locations ↔ Transactions)
- ✅ Use service discovery for inter-service communication
- ✅ Keep domain logic in core libraries, not in API projects
- ✅ Follow the existing layered structure when adding new projects
