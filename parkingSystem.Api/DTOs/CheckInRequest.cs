namespace parkingSystem.Api.DTOs;

public class CheckInRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public string SpotCode { get; set; } = string.Empty;
    public string VehicleType { get; set; } = "CAR";
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
}