using JetBrains.Annotations;

namespace SaHoFleet.Models.Vehicle;

/// <summary>
/// Telemetry data of a vehicle
/// </summary>
[UsedImplicitly]
public class VehicleTelemetry
{
    /// <summary>
    /// Unique identifier of the vehicle
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Current latitude location of the vehicle
    /// In EPSG:4326 
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Current latitude location of the vehicle
    /// In EPSG:4326 
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// Current speed of the vehicle
    /// In km/h
    /// </summary>
    public double Speed { get; set; }
}