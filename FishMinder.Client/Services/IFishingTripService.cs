using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service interface for managing fishing trips
/// </summary>
public interface IFishingTripService : IDataService<FishingTrip>
{
    /// <summary>
    /// Gets the current active trip
    /// </summary>
    Task<FishingTrip?> GetCurrentTripAsync();

    /// <summary>
    /// Starts a new fishing trip
    /// </summary>
    Task<FishingTrip> StartNewTripAsync(string tripName, string? location = null);

    /// <summary>
    /// Ends the current active trip
    /// </summary>
    Task<FishingTrip?> EndCurrentTripAsync();

    /// <summary>
    /// Adds an angler to the current trip
    /// </summary>
    Task<bool> AddAnglerToCurrentTripAsync(Angler angler);

    /// <summary>
    /// Removes an angler from the current trip
    /// </summary>
    Task<bool> RemoveAnglerFromCurrentTripAsync(Guid anglerId);

    /// <summary>
    /// Adds a catch to the current trip
    /// </summary>
    Task<bool> AddCatchToCurrentTripAsync(FishCatch fishCatch);

    /// <summary>
    /// Updates a catch in the current trip
    /// </summary>
    Task<bool> UpdateCatchInCurrentTripAsync(FishCatch fishCatch);

    /// <summary>
    /// Removes a catch from the current trip
    /// </summary>
    Task<bool> RemoveCatchFromCurrentTripAsync(Guid catchId);

    /// <summary>
    /// Gets all trips ordered by start time (most recent first)
    /// </summary>
    Task<IEnumerable<FishingTrip>> GetTripsOrderedByDateAsync();

    /// <summary>
    /// Gets trip statistics
    /// </summary>
    Task<TripStatistics> GetTripStatisticsAsync(Guid tripId);
}

/// <summary>
/// Statistics for a fishing trip
/// </summary>
public class TripStatistics
{
    public int TotalCatches { get; set; }
    public int TotalKept { get; set; }
    public int TotalReleased { get; set; }
    public int UniqueSpecies { get; set; }
    public int ActiveAnglers { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, int> CatchesBySpecies { get; set; } = new();
    public Dictionary<string, int> CatchesByAngler { get; set; } = new();
}
