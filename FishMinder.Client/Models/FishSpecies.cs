using System.ComponentModel.DataAnnotations;

namespace FishMinder.Client.Models;

/// <summary>
/// Represents a fish species with regional regulations
/// </summary>
public class FishSpecies
{
    /// <summary>
    /// Unique identifier for the species
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Common name of the fish species
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Scientific name of the species
    /// </summary>
    public string? ScientificName { get; set; }

    /// <summary>
    /// Region where this species configuration applies (MN, ND, etc.)
    /// </summary>
    [Required]
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Daily bag limit for this species in the region
    /// </summary>
    public int DailyLimit { get; set; } = 0;

    /// <summary>
    /// Minimum size limit in inches (0 if no minimum)
    /// </summary>
    public decimal MinimumSize { get; set; } = 0;

    /// <summary>
    /// Maximum size limit in inches (0 if no maximum)
    /// </summary>
    public decimal MaximumSize { get; set; } = 0;

    /// <summary>
    /// Whether this species is currently in season
    /// </summary>
    public bool IsInSeason { get; set; } = true;

    /// <summary>
    /// Season start date (null if year-round)
    /// </summary>
    public DateTime? SeasonStart { get; set; }

    /// <summary>
    /// Season end date (null if year-round)
    /// </summary>
    public DateTime? SeasonEnd { get; set; }

    /// <summary>
    /// Number of times this species has been used (for sorting)
    /// </summary>
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// Last time this species was used for a catch
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Additional notes or regulations for this species
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this is a custom species added by the user
    /// </summary>
    public bool IsCustom { get; set; } = false;

    public override string ToString() => Name;
}
