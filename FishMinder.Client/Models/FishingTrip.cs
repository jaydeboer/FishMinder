using System.ComponentModel.DataAnnotations;

namespace FishMinder.Client.Models;

/// <summary>
/// Represents a fishing trip session
/// </summary>
public class FishingTrip
{
    /// <summary>
    /// Unique identifier for this trip
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Name or title of the fishing trip
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// When the trip started
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the trip ended (null if still active)
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Location name or description
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// GPS coordinates of the primary fishing location
    /// </summary>
    public GpsCoordinate? PrimaryLocation { get; set; }

    /// <summary>
    /// List of anglers participating in this trip
    /// </summary>
    public List<Angler> Anglers { get; set; } = new();

    /// <summary>
    /// All fish catches during this trip
    /// </summary>
    public List<FishCatch> Catches { get; set; } = new();

    /// <summary>
    /// Weather conditions during the trip
    /// </summary>
    public string? WeatherConditions { get; set; }

    /// <summary>
    /// Water temperature range during the trip
    /// </summary>
    public string? WaterTemperature { get; set; }

    /// <summary>
    /// Wind conditions during the trip
    /// </summary>
    public string? WindConditions { get; set; }

    /// <summary>
    /// General notes about the trip
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this trip is currently active
    /// </summary>
    public bool IsActive => EndTime == null;

    /// <summary>
    /// Duration of the trip
    /// </summary>
    public TimeSpan Duration => (EndTime ?? DateTime.UtcNow) - StartTime;

    /// <summary>
    /// Total number of fish caught during this trip
    /// </summary>
    public int TotalCatches => Catches.Count;

    /// <summary>
    /// Number of fish kept during this trip
    /// </summary>
    public int FishKept => Catches.Count(c => c.CountsTowardLimit);

    /// <summary>
    /// Number of fish released during this trip
    /// </summary>
    public int FishReleased => Catches.Count(c => c.Disposition == FishDisposition.Released);

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets catches for a specific angler
    /// </summary>
    public IEnumerable<FishCatch> GetCatchesForAngler(Guid anglerId)
    {
        return Catches.Where(c => c.AnglerId == anglerId);
    }

    /// <summary>
    /// Gets catches for a specific species
    /// </summary>
    public IEnumerable<FishCatch> GetCatchesForSpecies(Guid speciesId)
    {
        return Catches.Where(c => c.SpeciesId == speciesId);
    }

    /// <summary>
    /// Gets count of kept fish for a specific species and angler
    /// </summary>
    public int GetKeptCountForSpeciesAndAngler(Guid speciesId, Guid anglerId)
    {
        return Catches.Count(c => c.SpeciesId == speciesId && 
                                 c.AnglerId == anglerId && 
                                 c.CountsTowardLimit);
    }

    /// <summary>
    /// Gets count of kept fish for a specific species across all anglers
    /// </summary>
    public int GetKeptCountForSpecies(Guid speciesId)
    {
        return Catches.Count(c => c.SpeciesId == speciesId && c.CountsTowardLimit);
    }
}
