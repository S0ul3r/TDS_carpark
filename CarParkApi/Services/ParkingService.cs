using CarParkApi.Data;
using CarParkApi.Enums;
using CarParkApi.Models;
using CarParkApi.Models.Requests;
using CarParkApi.Models.Responses;

namespace CarParkApi.Services;

public class ParkingService : IParkingService
{
    private readonly ICarParkRepository _repository;
    private readonly ILogger<ParkingService> _logger;

    // rates per minute
    private const decimal SmallCarRate = 0.10m;
    private const decimal MediumCarRate = 0.20m;
    private const decimal LargeCarRate = 0.40m;
    private const decimal AdditionalChargePerFiveMinutes = 1.00m;

    public ParkingService(ICarParkRepository repository, ILogger<ParkingService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ParkingResponse> ParkVehicleAsync(ParkVehicleRequest request)
    {
        _logger.LogInformation("Parking vehicle {VehicleReg}", request.VehicleReg);

        // Initial validation
        if (!Enum.TryParse<VehicleType>(request.VehicleType, true, out var vehicleType))
        {
            _logger.LogWarning("Invalid vehicle type: {VehicleType}", request.VehicleType);
            throw new ArgumentException($"Invalid vehicle type: {request.VehicleType}. Must be Small, Medium, or Large.");
        }

        // is alreayd parked?
        var isAlreadyParked = await _repository.IsVehicleAlreadyParkedAsync(request.VehicleReg);
        if (isAlreadyParked)
        {
            _logger.LogWarning("Vehicle {VehicleReg} is already parked", request.VehicleReg);
            throw new InvalidOperationException($"Vehicle {request.VehicleReg} is already parked.");
        }

        // check if have space
        var availableSpace = await _repository.GetAvailableSpaceAsync();
        if (availableSpace == null)
        {
            _logger.LogWarning("Car park full");
            throw new InvalidOperationException("Car park is full. No available spaces.");
        }

        availableSpace.VehicleReg = request.VehicleReg;
        availableSpace.VehicleType = vehicleType;
        availableSpace.TimeIn = DateTime.UtcNow;
        availableSpace.IsOccupied = true;

        var parkedSpace = await _repository.ParkVehicleAsync(availableSpace);

        _logger.LogInformation("Parked in space {SpaceNumber}", parkedSpace.SpaceNumber);

        return new ParkingResponse
        {
            VehicleReg = parkedSpace.VehicleReg!,
            SpaceNumber = parkedSpace.SpaceNumber,
            TimeIn = parkedSpace.TimeIn!.Value
        };
    }

    public async Task<SpaceStatusResponse> GetSpaceStatusAsync()
    {
        var availableSpaces = await _repository.GetAvailableSpacesCountAsync();
        var occupiedSpaces = await _repository.GetOccupiedSpacesCountAsync();

        return new SpaceStatusResponse
        {
            AvailableSpaces = availableSpaces,
            OccupiedSpaces = occupiedSpaces
        };
    }

    public async Task<ExitResponse> ProcessVehicleExitAsync(VehicleExitRequest request)
    {
        _logger.LogInformation("Processing exit for {VehicleReg}", request.VehicleReg);

        var parkingSpace = await _repository.GetParkingSpaceByVehicleRegAsync(request.VehicleReg);
        // check if is parked
        if (parkingSpace == null)
        {
            _logger.LogWarning("Vehicle not found: {VehicleReg}", request.VehicleReg);
            throw new KeyNotFoundException($"Vehicle {request.VehicleReg} is not currently parked.");
        }

        var timeOut = DateTime.UtcNow;
        var timeIn = parkingSpace.TimeIn!.Value;
        
        var charge = CalculateCharge(parkingSpace.VehicleType!.Value, timeIn, timeOut);

        _logger.LogInformation("Exit charge: £{Charge:F2}", charge);

        // free up the space
        parkingSpace.TimeOut = timeOut;
        parkingSpace.IsOccupied = false;
        parkingSpace.VehicleReg = null;
        parkingSpace.VehicleType = null;
        parkingSpace.TimeIn = null;

        await _repository.UpdateParkingSpaceAsync(parkingSpace);

        return new ExitResponse
        {
            VehicleReg = request.VehicleReg,
            VehicleCharge = (double)charge,
            TimeIn = timeIn,
            TimeOut = timeOut
        };
    }

    // charge calculation: (rate × minutes) + (£1 every 5min)
    private decimal CalculateCharge(VehicleType vehicleType, DateTime timeIn, DateTime timeOut)
    {
        var duration = timeOut - timeIn;
        var totalMinutes = (int)Math.Ceiling(duration.TotalMinutes);

        var ratePerMinute = vehicleType switch
        {
            VehicleType.Small => SmallCarRate,
            VehicleType.Medium => MediumCarRate,
            VehicleType.Large => LargeCarRate,
            _ => throw new ArgumentException($"Unknown vehicle type: {vehicleType}")
        };

        var baseCharge = ratePerMinute * totalMinutes;
        var additionalCharge = (totalMinutes / 5) * AdditionalChargePerFiveMinutes;
        var totalCharge = baseCharge + additionalCharge;

        _logger.LogDebug("{Minutes}min = £{BaseCharge:F2} + £{AdditionalCharge:F2} = £{TotalCharge:F2}",
            totalMinutes, baseCharge, additionalCharge, totalCharge);

        return Math.Round(totalCharge, 2);
    }
}

