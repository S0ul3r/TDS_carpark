using CarParkApi.Models;

namespace CarParkApi.Data;

public interface ICarParkRepository
{
    Task<ParkingSpace?> GetAvailableSpaceAsync();
    Task<ParkingSpace?> GetParkingSpaceByVehicleRegAsync(string vehicleReg);
    Task<int> GetAvailableSpacesCountAsync();
    Task<int> GetOccupiedSpacesCountAsync();
    Task<ParkingSpace> ParkVehicleAsync(ParkingSpace parkingSpace);
    Task<ParkingSpace> UpdateParkingSpaceAsync(ParkingSpace parkingSpace);
    Task<bool> IsVehicleAlreadyParkedAsync(string vehicleReg);
}

