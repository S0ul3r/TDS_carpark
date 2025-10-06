using CarParkApi.Models.Requests;
using CarParkApi.Models.Responses;

namespace CarParkApi.Services;

public interface IParkingService
{
    Task<ParkingResponse> ParkVehicleAsync(ParkVehicleRequest request);
    Task<SpaceStatusResponse> GetSpaceStatusAsync();
    Task<ExitResponse> ProcessVehicleExitAsync(VehicleExitRequest request);
}

