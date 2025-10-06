namespace CarParkApi.Models.Responses;

public class ExitResponse
{
    public string VehicleReg { get; set; } = string.Empty;
    public double VehicleCharge { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
}

