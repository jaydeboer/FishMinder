using Blazored.LocalStorage;
using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service for managing fishing trips with local storage
/// </summary>
public class FishingTripService : LocalStorageDataService<FishingTrip>, IFishingTripService
{
    private const string STORAGE_KEY = "fishminder_trips";
    private const string CURRENT_TRIP_KEY = "fishminder_current_trip_id";
    
    private readonly ILocalStorageService _localStorage;

    public FishingTripService(ILocalStorageService localStorage) 
        : base(localStorage, STORAGE_KEY)
    {
        _localStorage = localStorage;
    }

    public async Task<FishingTrip?> GetCurrentTripAsync()
    {
        try
        {
            var currentTripId = await _localStorage.GetItemAsync<Guid?>(CURRENT_TRIP_KEY);
            if (currentTripId.HasValue && currentTripId.Value != Guid.Empty)
            {
                var trip = await GetByIdAsync(currentTripId.Value);
                if (trip?.IsActive == true)
                {
                    return trip;
                }
                else
                {
                    // Clear invalid current trip reference
                    await _localStorage.RemoveItemAsync(CURRENT_TRIP_KEY);
                }
            }
        }
        catch
        {
            // Handle any storage errors gracefully
        }
        
        return null;
    }

    public async Task<FishingTrip> StartNewTripAsync(string tripName, string? location = null)
    {
        // End any existing active trip first
        await EndCurrentTripAsync();

        var newTrip = new FishingTrip
        {
            Id = Guid.NewGuid(),
            Name = tripName,
            Location = location,
            StartTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        await CreateAsync(newTrip);
        await _localStorage.SetItemAsync(CURRENT_TRIP_KEY, newTrip.Id);
        
        return newTrip;
    }

    public async Task<FishingTrip?> EndCurrentTripAsync()
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            currentTrip.EndTime = DateTime.UtcNow;
            currentTrip.ModifiedAt = DateTime.UtcNow;
            await UpdateAsync(currentTrip);
            await _localStorage.RemoveItemAsync(CURRENT_TRIP_KEY);
            return currentTrip;
        }
        
        return null;
    }

    public async Task<bool> AddAnglerToCurrentTripAsync(Angler angler)
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            // Check if angler already exists
            if (!currentTrip.Anglers.Any(a => a.Id == angler.Id))
            {
                angler.AddedToTrip = DateTime.UtcNow;
                currentTrip.Anglers.Add(angler);
                currentTrip.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(currentTrip);
                return true;
            }
        }
        
        return false;
    }

    public async Task<bool> RemoveAnglerFromCurrentTripAsync(Guid anglerId)
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            var angler = currentTrip.Anglers.FirstOrDefault(a => a.Id == anglerId);
            if (angler != null)
            {
                angler.IsActive = false;
                currentTrip.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(currentTrip);
                return true;
            }
        }
        
        return false;
    }

    public async Task<bool> AddCatchToCurrentTripAsync(FishCatch fishCatch)
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            fishCatch.Id = Guid.NewGuid();
            fishCatch.CreatedAt = DateTime.UtcNow;
            fishCatch.ModifiedAt = DateTime.UtcNow;
            
            currentTrip.Catches.Add(fishCatch);
            currentTrip.ModifiedAt = DateTime.UtcNow;
            await UpdateAsync(currentTrip);
            return true;
        }
        
        return false;
    }

    public async Task<bool> UpdateCatchInCurrentTripAsync(FishCatch fishCatch)
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            var index = currentTrip.Catches.FindIndex(c => c.Id == fishCatch.Id);
            if (index >= 0)
            {
                fishCatch.ModifiedAt = DateTime.UtcNow;
                currentTrip.Catches[index] = fishCatch;
                currentTrip.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(currentTrip);
                return true;
            }
        }
        
        return false;
    }

    public async Task<bool> RemoveCatchFromCurrentTripAsync(Guid catchId)
    {
        var currentTrip = await GetCurrentTripAsync();
        if (currentTrip != null)
        {
            var index = currentTrip.Catches.FindIndex(c => c.Id == catchId);
            if (index >= 0)
            {
                currentTrip.Catches.RemoveAt(index);
                currentTrip.ModifiedAt = DateTime.UtcNow;
                await UpdateAsync(currentTrip);
                return true;
            }
        }
        
        return false;
    }

    public async Task<IEnumerable<FishingTrip>> GetTripsOrderedByDateAsync()
    {
        var trips = await GetAllAsync();
        return trips.OrderByDescending(t => t.StartTime);
    }

    public async Task<TripStatistics> GetTripStatisticsAsync(Guid tripId)
    {
        var trip = await GetByIdAsync(tripId);
        if (trip == null)
        {
            return new TripStatistics();
        }

        var stats = new TripStatistics
        {
            TotalCatches = trip.TotalCatches,
            TotalKept = trip.FishKept,
            TotalReleased = trip.FishReleased,
            UniqueSpecies = trip.Catches.Select(c => c.SpeciesId).Distinct().Count(),
            ActiveAnglers = trip.Anglers.Count(a => a.IsActive),
            Duration = trip.Duration
        };

        // Group catches by species (would need species service to get names)
        stats.CatchesBySpecies = trip.Catches
            .GroupBy(c => c.SpeciesId.ToString()) // TODO: Replace with species name
            .ToDictionary(g => g.Key, g => g.Count());

        // Group catches by angler (would need angler names)
        stats.CatchesByAngler = trip.Catches
            .GroupBy(c => c.AnglerId)
            .ToDictionary(g => trip.Anglers.FirstOrDefault(a => a.Id == g.Key)?.Name ?? "Unknown", 
                         g => g.Count());

        return stats;
    }
}
