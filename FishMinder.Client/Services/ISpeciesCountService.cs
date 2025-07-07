using FishMinder.Client.Models;

namespace FishMinder.Client.Services;

/// <summary>
/// Service interface for managing species counts and limits
/// </summary>
public interface ISpeciesCountService
{
    /// <summary>
    /// Gets current species counts for the active trip
    /// </summary>
    Task<IEnumerable<SpeciesCount>> GetCurrentTripSpeciesCountsAsync();

    /// <summary>
    /// Gets species count for a specific species in the current trip
    /// </summary>
    Task<SpeciesCount?> GetSpeciesCountAsync(Guid speciesId);

    /// <summary>
    /// Gets species count for a specific angler and species in the current trip
    /// </summary>
    Task<SpeciesCount?> GetAnglerSpeciesCountAsync(Guid anglerId, Guid speciesId);

    /// <summary>
    /// Gets all species counts for a specific angler in the current trip
    /// </summary>
    Task<IEnumerable<SpeciesCount>> GetAnglerSpeciesCountsAsync(Guid anglerId);

    /// <summary>
    /// Checks if adding a catch would exceed daily limits
    /// </summary>
    Task<LimitCheckResult> CheckDailyLimitAsync(Guid anglerId, Guid speciesId, int additionalCount = 1);

    /// <summary>
    /// Checks if adding a catch would exceed possession limits
    /// </summary>
    Task<LimitCheckResult> CheckPossessionLimitAsync(Guid anglerId, Guid speciesId, int additionalCount = 1);

    /// <summary>
    /// Gets species that are approaching their limits (within warning threshold)
    /// </summary>
    Task<IEnumerable<SpeciesLimitWarning>> GetLimitWarningsAsync(Guid? anglerId = null);

    /// <summary>
    /// Gets species that have exceeded their limits
    /// </summary>
    Task<IEnumerable<SpeciesLimitWarning>> GetLimitExceedingsAsync(Guid? anglerId = null);

    /// <summary>
    /// Gets comprehensive trip statistics including species breakdowns
    /// </summary>
    Task<TripSpeciesStatistics> GetTripSpeciesStatisticsAsync();

    /// <summary>
    /// Gets historical species count data for analytics
    /// </summary>
    Task<IEnumerable<SpeciesCount>> GetHistoricalSpeciesCountsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Refreshes cached count data (call after adding/removing catches)
    /// </summary>
    Task RefreshCountsAsync();
}

/// <summary>
/// Represents species count data for a specific species
/// </summary>
public class SpeciesCount
{
    public Guid SpeciesId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public Guid? AnglerId { get; set; }
    public string? AnglerName { get; set; }
    public int TotalCaught { get; set; }
    public int TotalKept { get; set; }
    public int TotalReleased { get; set; }
    public int InLivewell { get; set; }
    public int Died { get; set; }
    public DateTime? FirstCatch { get; set; }
    public DateTime? LastCatch { get; set; }
    public decimal? LargestFish { get; set; }
    public decimal? SmallestFish { get; set; }
    public decimal? AverageSize { get; set; }
    public decimal? TotalWeight { get; set; }
    public int DailyLimit { get; set; }
    public int PossessionLimit { get; set; }
    public bool IsOverDailyLimit => DailyLimit > 0 && TotalKept > DailyLimit;
    public bool IsOverPossessionLimit => PossessionLimit > 0 && TotalKept > PossessionLimit;
    public bool IsApproachingDailyLimit => DailyLimit > 0 && TotalKept >= (DailyLimit * 0.8);
    public bool IsApproachingPossessionLimit => PossessionLimit > 0 && TotalKept >= (PossessionLimit * 0.8);
    public double DailyLimitPercentage => DailyLimit > 0 ? (double)TotalKept / DailyLimit * 100 : 0;
    public double PossessionLimitPercentage => PossessionLimit > 0 ? (double)TotalKept / PossessionLimit * 100 : 0;
}

/// <summary>
/// Result of a limit check operation
/// </summary>
public class LimitCheckResult
{
    public bool IsWithinLimit { get; set; }
    public bool IsApproachingLimit { get; set; }
    public int CurrentCount { get; set; }
    public int Limit { get; set; }
    public int RemainingCount { get; set; }
    public string Message { get; set; } = string.Empty;
    public LimitType LimitType { get; set; }
}

/// <summary>
/// Warning information for species approaching or exceeding limits
/// </summary>
public class SpeciesLimitWarning
{
    public Guid SpeciesId { get; set; }
    public string SpeciesName { get; set; } = string.Empty;
    public Guid? AnglerId { get; set; }
    public string? AnglerName { get; set; }
    public int CurrentCount { get; set; }
    public int Limit { get; set; }
    public LimitType LimitType { get; set; }
    public WarningLevel WarningLevel { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Comprehensive trip statistics with species breakdowns
/// </summary>
public class TripSpeciesStatistics
{
    public int TotalCatches { get; set; }
    public int TotalKept { get; set; }
    public int TotalReleased { get; set; }
    public int UniqueSpecies { get; set; }
    public int ActiveAnglers { get; set; }
    public TimeSpan TripDuration { get; set; }
    public IEnumerable<SpeciesCount> SpeciesCounts { get; set; } = new List<SpeciesCount>();
    public IEnumerable<AnglerStatistics> AnglerStats { get; set; } = new List<AnglerStatistics>();
    public DateTime? FirstCatch { get; set; }
    public DateTime? LastCatch { get; set; }
    public string? MostCaughtSpecies { get; set; }
    public string? TopAngler { get; set; }
}

/// <summary>
/// Statistics for individual anglers
/// </summary>
public class AnglerStatistics
{
    public Guid AnglerId { get; set; }
    public string AnglerName { get; set; } = string.Empty;
    public int TotalCatches { get; set; }
    public int TotalKept { get; set; }
    public int TotalReleased { get; set; }
    public int UniqueSpecies { get; set; }
    public decimal? LargestFish { get; set; }
    public string? LargestFishSpecies { get; set; }
    public IEnumerable<SpeciesCount> SpeciesCounts { get; set; } = new List<SpeciesCount>();
}

/// <summary>
/// Types of limits that can be checked
/// </summary>
public enum LimitType
{
    Daily,
    Possession
}

/// <summary>
/// Warning levels for limit notifications
/// </summary>
public enum WarningLevel
{
    None,
    Approaching,
    AtLimit,
    Exceeded
}
