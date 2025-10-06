namespace CarParkApi.Models.Responses;

public class ParkingResponse
{
    public string VehicleReg { get; set; } = string.Empty;
    public int SpaceNumber { get; set; }
    public DateTime TimeIn { get; set; }
}

