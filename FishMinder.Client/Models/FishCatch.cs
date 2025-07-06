using System.ComponentModel.DataAnnotations;

namespace FishMinder.Client.Models;

/// <summary>
/// Represents a fish catch record
/// </summary>
public class FishCatch
{
    /// <summary>
    /// Unique identifier for this catch
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the angler who caught this fish
    /// </summary>
    [Required]
    public Guid AnglerId { get; set; }

    /// <summary>
    /// ID of the fish species
    /// </summary>
    [Required]
    public Guid SpeciesId { get; set; }

    /// <summary>
    /// When this fish was caught
    /// </summary>
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// GPS coordinates where the fish was caught
    /// </summary>
    public GpsCoordinate? Location { get; set; }

    /// <summary>
    /// What happened to the fish after being caught
    /// </summary>
    [Required]
    public FishDisposition Disposition { get; set; } = FishDisposition.Released;

    /// <summary>
    /// Length of the fish in inches (optional)
    /// </summary>
    [Range(0, 100)]
    public decimal? Length { get; set; }

    /// <summary>
    /// Weight of the fish in pounds (optional)
    /// </summary>
    [Range(0, 100)]
    public decimal? Weight { get; set; }

    /// <summary>
    /// Optional notes about this catch
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Optional photo file path or URL
    /// </summary>
    public string? PhotoPath { get; set; }

    /// <summary>
    /// Bait or lure used to catch this fish
    /// </summary>
    public string? BaitUsed { get; set; }

    /// <summary>
    /// Water depth where fish was caught (in feet)
    /// </summary>
    [Range(0, 1000)]
    public decimal? WaterDepth { get; set; }

    /// <summary>
    /// Water temperature when fish was caught (in Fahrenheit)
    /// </summary>
    [Range(32, 100)]
    public decimal? WaterTemperature { get; set; }

    /// <summary>
    /// Weather conditions when fish was caught
    /// </summary>
    public string? WeatherConditions { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this record was last modified
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this catch counts toward daily limits
    /// </summary>
    public bool CountsTowardLimit => Disposition == FishDisposition.Kept || Disposition == FishDisposition.InLivewell;
}

/// <summary>
/// What happened to the fish after being caught
/// </summary>
public enum FishDisposition
{
    /// <summary>
    /// Fish was released back to the water
    /// </summary>
    Released = 0,

    /// <summary>
    /// Fish was kept and harvested
    /// </summary>
    Kept = 1,

    /// <summary>
    /// Fish is being kept alive in a livewell
    /// </summary>
    InLivewell = 2,

    /// <summary>
    /// Fish died and was discarded
    /// </summary>
    Died = 3
}
