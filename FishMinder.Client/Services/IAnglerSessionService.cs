using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service interface for managing angler session state during fishing trips
/// </summary>
public interface IAnglerSessionService
{
    /// <summary>
    /// Event fired when the active angler selection changes
    /// </summary>
    event EventHandler<AnglerSessionChangedEventArgs>? ActiveAnglerChanged;

    /// <summary>
    /// Gets the currently selected angler for catch assignment
    /// </summary>
    Task<Angler?> GetActiveAnglerAsync();

    /// <summary>
    /// Sets the active angler for catch assignment
    /// </summary>
    Task<bool> SetActiveAnglerAsync(Guid anglerId);

    /// <summary>
    /// Clears the active angler selection
    /// </summary>
    Task ClearActiveAnglerAsync();

    /// <summary>
    /// Gets all anglers currently marked as fishing
    /// </summary>
    Task<IEnumerable<Angler>> GetFishingAnglersAsync();

    /// <summary>
    /// Marks an angler as currently fishing
    /// </summary>
    Task<bool> SetAnglerFishingStatusAsync(Guid anglerId, bool isFishing);

    /// <summary>
    /// Gets the fishing status for a specific angler
    /// </summary>
    Task<bool> IsAnglerFishingAsync(Guid anglerId);

    /// <summary>
    /// Gets angler session statistics
    /// </summary>
    Task<AnglerSessionStats> GetSessionStatsAsync(Guid anglerId);

    /// <summary>
    /// Resets all angler session state (useful when starting/ending trips)
    /// </summary>
    Task ResetSessionStateAsync();
}

/// <summary>
/// Event arguments for angler session changes
/// </summary>
public class AnglerSessionChangedEventArgs : EventArgs
{
    public Angler? PreviousAngler { get; set; }
    public Angler? CurrentAngler { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Statistics for an angler's session
/// </summary>
public class AnglerSessionStats
{
    public Guid AnglerId { get; set; }
    public string AnglerName { get; set; } = string.Empty;
    public bool IsFishing { get; set; }
    public bool IsActiveForCatches { get; set; }
    public DateTime? FishingStartedAt { get; set; }
    public TimeSpan TotalFishingTime { get; set; }
    public int CatchesThisSession { get; set; }
    public int CatchesPerHour { get; set; }
    public DateTime? LastCatchAt { get; set; }
}
