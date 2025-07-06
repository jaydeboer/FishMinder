using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Generic interface for data access operations
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public interface IDataService<T> where T : class
{
    /// <summary>
    /// Gets all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new entity
    /// </summary>
    Task<T> CreateAsync(T entity);

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    Task<T> UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by ID
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if an entity exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Gets the count of entities
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Clears all entities
    /// </summary>
    Task ClearAllAsync();
}
