# Sitecore MVP Selections API
<img src="https://dev.azure.com/iva-sitecore/MVP%20Selections/_apis/build/status/Sitecore.Mvp.Selections.Api?branchName=main" alt="Build Status">
<img src="https://vsrm.dev.azure.com/iva-sitecore/_apis/public/Release/badge/8beacf1d-5967-406b-8c62-b3090af96d35/1/1" alt="Deployment Status"/>

## Solution Architecture Overview

### 1. Layered Structure

The solution is organized into several projects, each with a distinct responsibility:

- **Mvp.Selections.Api**  
  The main API layer. Exposes endpoints for managing MVP applications, users, contributions, reviews, and related entities. Implements business logic through service classes (e.g., `ApplicationService`, `UserService`, `ContributionService`).

- **Mvp.Selections.Client**  
  A .NET client library for consuming the API. Provides strongly-typed models and extension methods for easy integration into other .NET applications. Links to shared models from the API project.

- **Mvp.Selections.Domain**  
  Contains core domain models (e.g., `User`, `Application`, `Contribution`, `MvpType`, `Country`, etc.) and enums. Used across all layers to ensure consistency.

- **Mvp.Selections.Data**  
  Implements repository interfaces and data access logic. Repositories abstract the persistence layer and provide methods for querying and manipulating domain entities.

---

### 2. Key Architectural Patterns

- **Dependency Injection**  
  All services and repositories are injected via constructors, promoting loose coupling and testability.

- **Repository Pattern**  
  Data access is abstracted behind repository interfaces (e.g., `IApplicationRepository`). This allows for flexible data storage and easier unit testing.

- **Service Layer**  
  Business logic is encapsulated in service classes (e.g., `ApplicationService`, `UserService`). These coordinate between repositories and enforce business rules.

- **DTOs and Models**  
  API and client projects use shared models for requests and responses, ensuring type safety and reducing duplication.

---

### 3. Extensibility and Integration

- **NuGet Packaging**  
  The client library can be packaged and distributed via NuGet for easy reuse.

- **Configuration**  
  The client supports configuration via environment variables or settings files.

- **Token Providers**  
  Authentication is abstracted via token providers, allowing integration with various authentication mechanisms.

---

### 4. Typical Flow Example

1. A web application uses the client library to interact with the API.
2. The client authenticates requests using a token provider.
3. API controllers delegate requests to service classes.
4. Services use repositories to fetch or persist data.
5. Domain models are used throughout for consistency.

---

### 5. Technology Stack

- **.NET 8 / C# 12**
- **ASP.NET Core** (for API)
- **Microsoft.Extensions** (for DI, configuration)
- **Entity Framework Core** (likely for data access, based on repository pattern)
- **NuGet** (for packaging/distribution)

---

**Summary:**  
The solution follows a clean, layered architecture with separation of concerns between API, client, domain, and data layers. It uses modern .NET practices for dependency injection, configuration, and extensibility, making it maintainable and scalable.