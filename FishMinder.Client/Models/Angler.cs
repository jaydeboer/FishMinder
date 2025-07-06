using System.ComponentModel.DataAnnotations;

namespace FishMinder.Client.Models;

/// <summary>
/// Represents an angler participating in a fishing trip
/// </summary>
public class Angler
{
    /// <summary>
    /// Unique identifier for the angler
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Display name of the angler
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional email address for the angler
    /// </summary>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// When this angler was added to the current trip
    /// </summary>
    public DateTime AddedToTrip { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this angler is currently active in the trip
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes about the angler
    /// </summary>
    public string? Notes { get; set; }

    public override string ToString() => Name;
}
