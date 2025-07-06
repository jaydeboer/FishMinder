using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service interface for managing fish species
/// </summary>
public interface IFishSpeciesService : IDataService<FishSpecies>
{
    /// <summary>
    /// Gets species sorted by usage (most recent first, then alphabetical)
    /// </summary>
    Task<IEnumerable<FishSpecies>> GetSpeciesSortedByUsageAsync();

    /// <summary>
    /// Gets species for a specific region
    /// </summary>
    Task<IEnumerable<FishSpecies>> GetSpeciesByRegionAsync(string region);

    /// <summary>
    /// Records usage of a species (updates usage count and last used date)
    /// </summary>
    Task RecordSpeciesUsageAsync(Guid speciesId);

    /// <summary>
    /// Gets species that are currently in season
    /// </summary>
    Task<IEnumerable<FishSpecies>> GetInSeasonSpeciesAsync();

    /// <summary>
    /// Initializes the database with default Minnesota and North Dakota species
    /// </summary>
    Task InitializeDefaultSpeciesAsync();

    /// <summary>
    /// Searches species by name
    /// </summary>
    Task<IEnumerable<FishSpecies>> SearchSpeciesByNameAsync(string searchTerm);

    /// <summary>
    /// Gets the most commonly used species (top N)
    /// </summary>
    Task<IEnumerable<FishSpecies>> GetMostUsedSpeciesAsync(int count = 10);

    /// <summary>
    /// Checks if species database needs initialization
    /// </summary>
    Task<bool> NeedsInitializationAsync();
}
