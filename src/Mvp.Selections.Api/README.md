# Architecture Overview

`Mvp.Selections.Api` is a modular, layered .NET 8 application designed to power the Sitecore MVP selection and review process. It is built as an Azure Functions Isolated Worker, leveraging dependency injection and modern .NET practices.

## Key Architectural Components

- **Entry Point & Hosting**
  - The application starts in `Program.cs`, configuring the host for Azure Functions and setting up all dependencies using the built-in DI container.

- **Dependency Injection**
  - Services, repositories, helpers, and options are registered via DI. This enables loose coupling and testability.

- **Service Layer**
  - Business logic is encapsulated in service classes (e.g., `ApplicationService`, `UserService`, `ScoreCardService`), each implementing corresponding interfaces (e.g., `IApplicationService`).
  - Services interact with repositories for data access and may implement multiple interfaces for cross-cutting concerns (e.g., `ApplicationService` implements both `IApplicationService` and `IApplicantService`).

- **Repository Layer**
  - Data access is abstracted via repository interfaces (e.g., `IApplicationRepository`) and their implementations.
  - Entity Framework Core is used for database operations, with context pooling for performance.

- **Domain Model**
  - Core entities (e.g., `Application`, `User`, `Country`, `MvpType`, `Selection`) are defined in the domain layer and used throughout the API.
  - Models for API responses (e.g., `Applicant`, `Mentor`, `ScoreCard`) are mapped from domain entities.

- **Options & Configuration**
  - Strongly-typed options classes are bound to configuration sections for external services, database connections, and feature flags.

- **Helpers & Utilities**
  - Utility classes (e.g., `AvatarUriHelper`, `CacheManager`) provide supporting functionality.

- **External Integrations**
  - HTTP clients are registered for external services (e.g., Okta, SearchIngestion, Community, SendClient).

- **Telemetry & Monitoring**
  - Application Insights is integrated for logging and monitoring.

## Design Patterns & Practices

- **Interface-based Design:** All services and repositories are accessed via interfaces, promoting abstraction and testability.
- **Separation of Concerns:** Business logic, data access, and API models are clearly separated.
- **Dependency Injection:** All dependencies are injected, following SOLID principles.
- **Asynchronous Programming:** All service and repository methods are asynchronous for scalability.

## Example Flow

1. **API Request** → Azure Function triggers the appropriate service.
2. **Service Layer** → Handles business logic, validates input, and interacts with repositories.
3. **Repository Layer** → Fetches or persists data using Entity Framework Core.
4. **Domain Model** → Entities are mapped to API models for response.
5. **Response** → Returned to the client.

## Extensibility

- New features can be added by creating new services and repositories, registering them in DI, and exposing them via Azure Functions.
- Configuration-driven options allow easy integration with new external systems.

## Summary  
`Mvp.Selections.Api` is a clean, layered, and extensible .NET API, following best practices for maintainability, scalability, and testability, with a strong focus on separation of concerns and dependency injection.

---

## Generic Request Handling Pattern

`Mvp.Selections.Api` employs a consistent, layered approach for handling API requests, particularly within its Azure Functions endpoints. The pattern is as follows:

1. **Azure Function Trigger**  
   Each API endpoint is implemented as an Azure Function, triggered by HTTP requests. The function signature specifies the HTTP method, route, and authorization level.

2. **Dependency Injection**  
   All required services (e.g., logging, serialization, authentication, business services) are injected via the constructor, promoting loose coupling and adherence to SOLID principles.

3. **Base Class Abstraction**  
   Endpoints inherit from a generic `Base<TLogger>` class, which provides shared logic for:
   - Security validation
   - Error handling
   - Response formatting

4. **Security & Validation**  
   Each request is wrapped in a call to `ExecuteSafeSecurityValidatedAsync`, which:
   - Validates the request and user rights using the injected `IAuthService`
   - Handles authentication and authorization
   - Catches and logs exceptions

5. **Business Logic Delegation**  
   Upon successful validation, the endpoint delegates to the appropriate service (e.g., `ISelectionService`, `IProductService`) to perform business operations asynchronously.

6. **Serialization & Response**  
   Results are serialized using the injected `ISerializer` and returned as an `IActionResult`, with appropriate status codes and contract resolvers for formatting.

7. **Error Handling**  
   Any errors or exceptions are caught, logged, and returned as standardized error responses.

### Example Flow
```csharp
return ExecuteSafeSecurityValidatedAsync(req, [Right.Admin], async _ => 
{
    OperationResult result = await service.DoSomethingAsync();
	return ContentResult(result);
});
```

### Summary  
Requests are processed through a secure, validated, and exception-safe pipeline, with business logic separated into services and responses consistently formatted. This pattern ensures maintainability, testability, and scalability across all API endpoints.

---

## Secure Serialization/Deserialization

- **Custom Serialization Settings:**  
  The API uses a custom `JsonOptions` class to configure `JsonSerializerSettings` for all JSON operations. Key security-related settings include:
  - `TypeNameHandling = Auto`: Enables polymorphic deserialization, but is restricted for safety.
  - **Custom SerializationBinder:**  
    The `MvpSelectionsDomainSerializationBinder` strictly whitelists types for deserialization. Only types from the domain assembly (such as those inheriting from `BaseEntity<>`) are allowed. This prevents attackers from injecting malicious types and executing arbitrary code during deserialization.
    ```csharp
    if (assemblyName?.Equals(typeof(BaseEntity<>).Assembly.GetName().Name) ?? false)
    {
        result = base.BindToType(assemblyName, typeName);
    }
    ```
    _This is a critical defense against code execution attacks via JSON payloads._

- **Contract Resolvers:**  
  Custom contract resolvers (e.g., `RolesContractResolver`) are used to exclude sensitive properties from serialization, ensuring only safe, intended data is exposed in API responses.
