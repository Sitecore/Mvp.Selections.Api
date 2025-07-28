# Mvp.Selections.Data Architecture

`Mvp.Selections.Data` is the data access layer for the MVP Selections application, built with C# 12 and targeting .NET 8. Its primary responsibilities are to manage database interactions, entity mapping, and repository logic.

## Key Components

### 1. DbContext (`Context`)
- Centralizes all entity sets (tables) using Entity Framework Core.
- Configures entity relationships, keys, indexes, and seed data in `OnModelCreating`.
- Exposes `DbSet<T>` properties for all domain entities (e.g., `Users`, `Roles`, `Consents`, `Selections`, etc.).

### 2. Entities
- Domain models (e.g., `User`, `Role`, `Consent`, `Selection`, `MvpType`, `Region`, etc.) are defined in the `Mvp.Selections.Domain` project and referenced here.
- Each entity is mapped to a table and relationships are configured in the context.

### 3. Repositories
- Each entity has a corresponding repository (e.g., `UserRepository`, `RegionRepository`, `MvpTypeRepository`, etc.).
- Repositories inherit from a generic `BaseRepository<TEntity, TId>`, which provides CRUD operations and audit property management.
- Some repositories (e.g., `ConsentRepository`) implement custom query methods for more complex data retrieval.

### 4. Interfaces
- Repository interfaces (e.g., `IUserRepository`, `IRegionRepository`) define contracts for data access.
- The `ICurrentUserNameProvider` interface is used for audit tracking (who performed changes).

### 5. Dependency Injection
- Repositories and the `Context` are registered in the DI container for use throughout the application (see `Program.cs` in the API project).

## Data Flow

- The application interacts with repositories via interfaces.
- Repositories use the `Context` to query and persist entities.
- Entity relationships and constraints are enforced by Entity Framework Core.
- Audit information (created/modified by) is managed via the `ICurrentUserNameProvider`.

## Extensibility

- New entities can be added by extending the domain model and updating the `Context`.
- Custom queries or business logic can be implemented in specific repositories.

## Summary 
`Mvp.Selections.Data` cleanly separates data access concerns using Entity Framework Core, generic repositories, and dependency injection. It provides a robust, testable, and maintainable foundation for all database operations in the MVP Selections solution.

---

## Repository Pattern in this Project

The repository pattern is used to abstract data access logic and provide a clean separation between the domain and data layers. Here’s how it is implemented:

### 1. Repository Interfaces

- Each domain entity (e.g., `Country`, `Review`, `Consent`, `Dispatch`, `ScoreCategory`) has a corresponding repository interface, such as `ICountryRepository`, `IReviewRepository`, etc.
- These interfaces inherit from a generic `IBaseRepository<TEntity, TId>`, which defines common CRUD operations:
  - Examples: `GetAsync`, `GetAllAsync`, `Add`, `RemoveAsync`, `SaveChangesAsync`.
- Interfaces may also declare additional, entity-specific methods (e.g., `GetAllForUserAsync` in `IConsentRepository`, `GetLast24HourAsync` in `IDispatchRepository`).

### 2. Repository Implementations

- Concrete repository classes (e.g., `ConsentRepository`, `DispatchRepository`, `ScoreCategoryRepository`) implement their respective interfaces.
- They inherit from a generic `BaseRepository<TEntity, TId>`, which provides the base implementation for common operations.
- Entity-specific methods are implemented using Entity Framework Core for querying the database.

### 3. Usage and Integration

- Repositories are injected into service classes via constructor injection (using **Dependency Injection**).
- Service classes use repositories to fetch, update, or delete domain entities, encapsulating business logic and coordinating data access.
- This abstraction allows for easier unit testing and flexibility in changing the data source without affecting business logic.

### 4. Example
```csharp
public interface IConsentRepository : IBaseRepository<Consent, Guid>
{
    Task<IList<Consent>> GetAllForUserAsync(Guid userId);
    Task<Consent?> GetForUserAsync(Guid userId, ConsentType type);
}
public class ConsentRepository : BaseRepository<Consent, Guid>, IConsentRepository
{
    public async Task<IList<Consent>> GetAllForUserAsync(Guid userId)
	{
      return await Context.Consents.Where(c => c.User.Id == userId).ToListAsync();
	}
	// Other methods...
}
```

### 5. Benefits

- **Loose Coupling:** Business logic is decoupled from data access.
- **Testability:** Repositories can be mocked for unit testing.
- **Extensibility:** New data sources or query logic can be added with minimal impact.

### Summary  
The solution uses a generic, interface-driven repository pattern with Entity Framework Core, promoting maintainability, testability, and separation of concerns between business logic and data access.