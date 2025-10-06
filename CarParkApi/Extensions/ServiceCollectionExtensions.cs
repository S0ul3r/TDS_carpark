using CarParkApi.Data;
using CarParkApi.Services;

namespace CarParkApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarParkServices(this IServiceCollection services)
    {
        services.AddScoped<ICarParkRepository, CarParkRepository>();
        services.AddScoped<IParkingService, ParkingService>();

        return services;
    }
}

