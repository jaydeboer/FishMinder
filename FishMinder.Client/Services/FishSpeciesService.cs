using Blazored.LocalStorage;
using FishMinder.Client.Data;
using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service for managing fish species with local storage
/// </summary>
public class FishSpeciesService : LocalStorageDataService<FishSpecies>, IFishSpeciesService
{
    private const string STORAGE_KEY = "fishminder_species";
    
    public FishSpeciesService(ILocalStorageService localStorage) 
        : base(localStorage, STORAGE_KEY)
    {
    }

    public async Task<IEnumerable<FishSpecies>> GetSpeciesSortedByUsageAsync()
    {
        var species = await GetAllAsync();
        
        return species
            .OrderByDescending(s => s.LastUsed ?? DateTime.MinValue)
            .ThenByDescending(s => s.UsageCount)
            .ThenBy(s => s.Name);
    }

    public async Task<IEnumerable<FishSpecies>> GetSpeciesByRegionAsync(string region)
    {
        var species = await GetAllAsync();
        return species.Where(s => s.Region.Equals(region, StringComparison.OrdinalIgnoreCase));
    }

    public async Task RecordSpeciesUsageAsync(Guid speciesId)
    {
        var species = await GetByIdAsync(speciesId);
        if (species != null)
        {
            species.UsageCount++;
            species.LastUsed = DateTime.UtcNow;
            await UpdateAsync(species);
        }
    }

    public async Task<IEnumerable<FishSpecies>> GetInSeasonSpeciesAsync()
    {
        var species = await GetAllAsync();
        var now = DateTime.UtcNow;
        
        return species.Where(s => s.IsInSeason && 
                                 (s.SeasonStart == null || s.SeasonStart <= now) &&
                                 (s.SeasonEnd == null || s.SeasonEnd >= now));
    }

    public async Task InitializeDefaultSpeciesAsync()
    {
        var existingCount = await CountAsync();
        if (existingCount == 0)
        {
            var defaultSpecies = DefaultSpeciesData.GetDefaultSpecies();
            
            foreach (var species in defaultSpecies)
            {
                await CreateAsync(species);
            }
        }
    }

    public async Task<IEnumerable<FishSpecies>> SearchSpeciesByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetSpeciesSortedByUsageAsync();
        }
        
        var species = await GetAllAsync();
        var searchLower = searchTerm.ToLowerInvariant();
        
        return species
            .Where(s => s.Name.ToLowerInvariant().Contains(searchLower) ||
                       (s.ScientificName?.ToLowerInvariant().Contains(searchLower) == true))
            .OrderBy(s => s.Name);
    }

    public async Task<IEnumerable<FishSpecies>> GetMostUsedSpeciesAsync(int count = 10)
    {
        var species = await GetAllAsync();
        
        return species
            .Where(s => s.UsageCount > 0)
            .OrderByDescending(s => s.UsageCount)
            .ThenByDescending(s => s.LastUsed)
            .Take(count);
    }

    public async Task<bool> NeedsInitializationAsync()
    {
        var count = await CountAsync();
        return count == 0;
    }
}
