using CarParkApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarParkApi.Data;

public class CarParkRepository : ICarParkRepository
{
    private readonly CarParkDbContext _context;

    public CarParkRepository(CarParkDbContext context)
    {
        _context = context;
    }

    public async Task<ParkingSpace?> GetAvailableSpaceAsync()
    {
        return await _context.ParkingSpaces
            .Where(ps => !ps.IsOccupied)
            .OrderBy(ps => ps.SpaceNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<ParkingSpace?> GetParkingSpaceByVehicleRegAsync(string vehicleReg)
    {
        return await _context.ParkingSpaces
            .FirstOrDefaultAsync(ps => ps.VehicleReg == vehicleReg && ps.IsOccupied);
    }

    public async Task<int> GetAvailableSpacesCountAsync()
    {
        return await _context.ParkingSpaces
            .CountAsync(ps => !ps.IsOccupied);
    }

    public async Task<int> GetOccupiedSpacesCountAsync()
    {
        return await _context.ParkingSpaces
            .CountAsync(ps => ps.IsOccupied);
    }

    public async Task<ParkingSpace> ParkVehicleAsync(ParkingSpace parkingSpace)
    {
        return await UpdateParkingSpaceAsync(parkingSpace);
    }

    public async Task<ParkingSpace> UpdateParkingSpaceAsync(ParkingSpace parkingSpace)
    {
        _context.ParkingSpaces.Update(parkingSpace);
        await _context.SaveChangesAsync();
        return parkingSpace;
    }

    public async Task<bool> IsVehicleAlreadyParkedAsync(string vehicleReg)
    {
        return await _context.ParkingSpaces
            .AnyAsync(ps => ps.VehicleReg == vehicleReg && ps.IsOccupied);
    }
}

