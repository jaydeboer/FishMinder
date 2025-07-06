using Blazored.LocalStorage;
using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service for managing angler session state during fishing trips
/// </summary>
public class AnglerSessionService : IAnglerSessionService
{
    private const string ACTIVE_ANGLER_KEY = "fishminder_active_angler_id";
    private const string FISHING_ANGLERS_KEY = "fishminder_fishing_anglers";
    private const string SESSION_STATS_KEY = "fishminder_session_stats";

    private readonly ILocalStorageService _localStorage;
    private readonly IFishingTripService _tripService;

    public event EventHandler<AnglerSessionChangedEventArgs>? ActiveAnglerChanged;

    public AnglerSessionService(ILocalStorageService localStorage, IFishingTripService tripService)
    {
        _localStorage = localStorage;
        _tripService = tripService;
    }

    public async Task<Angler?> GetActiveAnglerAsync()
    {
        try
        {
            var activeAnglerId = await _localStorage.GetItemAsync<Guid?>(ACTIVE_ANGLER_KEY);
            if (activeAnglerId.HasValue && activeAnglerId.Value != Guid.Empty)
            {
                var currentTrip = await _tripService.GetCurrentTripAsync();
                if (currentTrip != null)
                {
                    return currentTrip.Anglers.FirstOrDefault(a => a.Id == activeAnglerId.Value && a.IsActive);
                }
            }
        }
        catch
        {
            // Handle storage errors gracefully
        }

        return null;
    }

    public async Task<bool> SetActiveAnglerAsync(Guid anglerId)
    {
        try
        {
            var currentTrip = await _tripService.GetCurrentTripAsync();
            if (currentTrip == null)
                return false;

            var angler = currentTrip.Anglers.FirstOrDefault(a => a.Id == anglerId && a.IsActive);
            if (angler == null)
                return false;

            var previousAngler = await GetActiveAnglerAsync();
            
            await _localStorage.SetItemAsync(ACTIVE_ANGLER_KEY, anglerId);

            // Fire event
            ActiveAnglerChanged?.Invoke(this, new AnglerSessionChangedEventArgs
            {
                PreviousAngler = previousAngler,
                CurrentAngler = angler
            });

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task ClearActiveAnglerAsync()
    {
        try
        {
            var previousAngler = await GetActiveAnglerAsync();
            await _localStorage.RemoveItemAsync(ACTIVE_ANGLER_KEY);

            // Fire event
            ActiveAnglerChanged?.Invoke(this, new AnglerSessionChangedEventArgs
            {
                PreviousAngler = previousAngler,
                CurrentAngler = null
            });
        }
        catch
        {
            // Handle storage errors gracefully
        }
    }

    public async Task<IEnumerable<Angler>> GetFishingAnglersAsync()
    {
        try
        {
            var fishingAnglerIds = await _localStorage.GetItemAsync<List<Guid>>(FISHING_ANGLERS_KEY) ?? new List<Guid>();
            var currentTrip = await _tripService.GetCurrentTripAsync();
            
            if (currentTrip != null)
            {
                return currentTrip.Anglers.Where(a => a.IsActive && fishingAnglerIds.Contains(a.Id));
            }
        }
        catch
        {
            // Handle storage errors gracefully
        }

        return Enumerable.Empty<Angler>();
    }

    public async Task<bool> SetAnglerFishingStatusAsync(Guid anglerId, bool isFishing)
    {
        try
        {
            var currentTrip = await _tripService.GetCurrentTripAsync();
            if (currentTrip == null)
                return false;

            var angler = currentTrip.Anglers.FirstOrDefault(a => a.Id == anglerId && a.IsActive);
            if (angler == null)
                return false;

            var fishingAnglerIds = await _localStorage.GetItemAsync<List<Guid>>(FISHING_ANGLERS_KEY) ?? new List<Guid>();

            if (isFishing)
            {
                if (!fishingAnglerIds.Contains(anglerId))
                {
                    fishingAnglerIds.Add(anglerId);
                    await UpdateSessionStats(anglerId, true);
                }
            }
            else
            {
                fishingAnglerIds.Remove(anglerId);
                await UpdateSessionStats(anglerId, false);
            }

            await _localStorage.SetItemAsync(FISHING_ANGLERS_KEY, fishingAnglerIds);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsAnglerFishingAsync(Guid anglerId)
    {
        try
        {
            var fishingAnglerIds = await _localStorage.GetItemAsync<List<Guid>>(FISHING_ANGLERS_KEY) ?? new List<Guid>();
            return fishingAnglerIds.Contains(anglerId);
        }
        catch
        {
            return false;
        }
    }

    public async Task<AnglerSessionStats> GetSessionStatsAsync(Guid anglerId)
    {
        try
        {
            var currentTrip = await _tripService.GetCurrentTripAsync();
            var angler = currentTrip?.Anglers.FirstOrDefault(a => a.Id == anglerId);
            
            if (angler == null)
            {
                return new AnglerSessionStats { AnglerId = anglerId };
            }

            var sessionStatsDict = await _localStorage.GetItemAsync<Dictionary<string, AnglerSessionStats>>(SESSION_STATS_KEY) 
                                   ?? new Dictionary<string, AnglerSessionStats>();

            var key = anglerId.ToString();
            if (!sessionStatsDict.ContainsKey(key))
            {
                sessionStatsDict[key] = new AnglerSessionStats
                {
                    AnglerId = anglerId,
                    AnglerName = angler.Name
                };
            }

            var stats = sessionStatsDict[key];
            stats.AnglerName = angler.Name; // Update name in case it changed
            stats.IsFishing = await IsAnglerFishingAsync(anglerId);
            stats.IsActiveForCatches = (await GetActiveAnglerAsync())?.Id == anglerId;

            // Calculate catches this session
            if (currentTrip != null)
            {
                var catches = currentTrip.GetCatchesForAngler(anglerId);
                stats.CatchesThisSession = catches.Count();
                stats.LastCatchAt = catches.OrderByDescending(c => c.CaughtAt).FirstOrDefault()?.CaughtAt;

                // Calculate catches per hour
                if (stats.TotalFishingTime.TotalHours > 0)
                {
                    stats.CatchesPerHour = (int)(stats.CatchesThisSession / stats.TotalFishingTime.TotalHours);
                }
            }

            return stats;
        }
        catch
        {
            return new AnglerSessionStats { AnglerId = anglerId };
        }
    }

    public async Task ResetSessionStateAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync(ACTIVE_ANGLER_KEY);
            await _localStorage.RemoveItemAsync(FISHING_ANGLERS_KEY);
            await _localStorage.RemoveItemAsync(SESSION_STATS_KEY);

            // Fire event to clear active angler
            ActiveAnglerChanged?.Invoke(this, new AnglerSessionChangedEventArgs
            {
                PreviousAngler = null,
                CurrentAngler = null
            });
        }
        catch
        {
            // Handle storage errors gracefully
        }
    }

    private async Task UpdateSessionStats(Guid anglerId, bool startedFishing)
    {
        try
        {
            var sessionStatsDict = await _localStorage.GetItemAsync<Dictionary<string, AnglerSessionStats>>(SESSION_STATS_KEY) 
                                   ?? new Dictionary<string, AnglerSessionStats>();

            var key = anglerId.ToString();
            if (!sessionStatsDict.ContainsKey(key))
            {
                sessionStatsDict[key] = new AnglerSessionStats
                {
                    AnglerId = anglerId
                };
            }

            var stats = sessionStatsDict[key];
            var now = DateTime.UtcNow;

            if (startedFishing)
            {
                stats.FishingStartedAt = now;
            }
            else if (stats.FishingStartedAt.HasValue)
            {
                var sessionTime = now - stats.FishingStartedAt.Value;
                stats.TotalFishingTime = stats.TotalFishingTime.Add(sessionTime);
                stats.FishingStartedAt = null;
            }

            await _localStorage.SetItemAsync(SESSION_STATS_KEY, sessionStatsDict);
        }
        catch
        {
            // Handle storage errors gracefully
        }
    }
}
