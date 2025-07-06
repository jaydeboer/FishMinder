using System.ComponentModel.DataAnnotations;

namespace FishMinder.Client.Models;

/// <summary>
/// Represents GPS coordinates with accuracy information
/// </summary>
public class GpsCoordinate
{
    /// <summary>
    /// Latitude in decimal degrees
    /// </summary>
    [Range(-90.0, 90.0)]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees
    /// </summary>
    [Range(-180.0, 180.0)]
    public double Longitude { get; set; }

    /// <summary>
    /// Accuracy of the GPS reading in meters
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Altitude in meters above sea level
    /// </summary>
    public double? Altitude { get; set; }

    /// <summary>
    /// When this coordinate was captured
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this coordinate was manually entered or GPS captured
    /// </summary>
    public bool IsManualEntry { get; set; } = false;

    /// <summary>
    /// Creates a new GPS coordinate
    /// </summary>
    public GpsCoordinate() { }

    /// <summary>
    /// Creates a new GPS coordinate with latitude and longitude
    /// </summary>
    public GpsCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Creates a new GPS coordinate with full details
    /// </summary>
    public GpsCoordinate(double latitude, double longitude, double? accuracy, double? altitude = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Accuracy = accuracy;
        Altitude = altitude;
    }

    /// <summary>
    /// Returns a formatted string representation of the coordinates
    /// </summary>
    public override string ToString()
    {
        return $"{Latitude:F6}, {Longitude:F6}";
    }

    /// <summary>
    /// Returns coordinates in degrees, minutes, seconds format
    /// </summary>
    public string ToDmsFormat()
    {
        var latDms = ConvertToDms(Latitude, true);
        var lonDms = ConvertToDms(Longitude, false);
        return $"{latDms}, {lonDms}";
    }

    private static string ConvertToDms(double coordinate, bool isLatitude)
    {
        var direction = coordinate >= 0 
            ? (isLatitude ? "N" : "E") 
            : (isLatitude ? "S" : "W");
        
        var absolute = Math.Abs(coordinate);
        var degrees = (int)absolute;
        var minutes = (int)((absolute - degrees) * 60);
        var seconds = ((absolute - degrees) * 60 - minutes) * 60;
        
        return $"{degrees}°{minutes}'{seconds:F2}\"{direction}";
    }
}
