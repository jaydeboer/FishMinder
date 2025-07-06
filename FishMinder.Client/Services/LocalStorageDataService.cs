using Blazored.LocalStorage;
using System.Text.Json;
using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Generic local storage data service implementation
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public class LocalStorageDataService<T> : IDataService<T> where T : class
{
    private readonly ILocalStorageService _localStorage;
    private readonly string _storageKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public LocalStorageDataService(ILocalStorageService localStorage, string storageKey)
    {
        _localStorage = localStorage;
        _storageKey = storageKey;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            var items = await _localStorage.GetItemAsync<List<T>>(_storageKey);
            return items ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(item => GetEntityId(item) == id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        var items = (await GetAllAsync()).ToList();
        
        // Ensure the entity has an ID
        if (GetEntityId(entity) == Guid.Empty)
        {
            SetEntityId(entity, Guid.NewGuid());
        }
        
        // Set timestamps if the entity supports them
        SetTimestamps(entity, true);
        
        items.Add(entity);
        await SaveAllAsync(items);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        var items = (await GetAllAsync()).ToList();
        var entityId = GetEntityId(entity);
        var index = items.FindIndex(item => GetEntityId(item) == entityId);
        
        if (index >= 0)
        {
            // Set modified timestamp if the entity supports it
            SetTimestamps(entity, false);
            items[index] = entity;
            await SaveAllAsync(items);
        }
        
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var items = (await GetAllAsync()).ToList();
        var index = items.FindIndex(item => GetEntityId(item) == id);
        
        if (index >= 0)
        {
            items.RemoveAt(index);
            await SaveAllAsync(items);
            return true;
        }
        
        return false;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        return item != null;
    }

    public async Task<int> CountAsync()
    {
        var items = await GetAllAsync();
        return items.Count();
    }

    public async Task ClearAllAsync()
    {
        await _localStorage.RemoveItemAsync(_storageKey);
    }

    private async Task SaveAllAsync(List<T> items)
    {
        await _localStorage.SetItemAsync(_storageKey, items);
    }

    private static Guid GetEntityId(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty?.PropertyType == typeof(Guid))
        {
            return (Guid)(idProperty.GetValue(entity) ?? Guid.Empty);
        }
        return Guid.Empty;
    }

    private static void SetEntityId(T entity, Guid id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty?.PropertyType == typeof(Guid) && idProperty.CanWrite)
        {
            idProperty.SetValue(entity, id);
        }
    }

    private static void SetTimestamps(T entity, bool isCreate)
    {
        var now = DateTime.UtcNow;
        
        if (isCreate)
        {
            var createdAtProperty = typeof(T).GetProperty("CreatedAt");
            if (createdAtProperty?.PropertyType == typeof(DateTime) && createdAtProperty.CanWrite)
            {
                createdAtProperty.SetValue(entity, now);
            }
        }
        
        var modifiedAtProperty = typeof(T).GetProperty("ModifiedAt");
        if (modifiedAtProperty?.PropertyType == typeof(DateTime) && modifiedAtProperty.CanWrite)
        {
            modifiedAtProperty.SetValue(entity, now);
        }
    }
}
