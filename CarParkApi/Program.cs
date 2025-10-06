using CarParkApi.Data;
using CarParkApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS - fix swagger issues
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure PostgreSQL database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<CarParkDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register application services
builder.Services.AddCarParkServices();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Car Park Management API",
        Version = "v1",
        Description = @"API for managing car park operations.

**Pricing:**
- Small car: £0.10/min
- Medium car: £0.20/min
- Large car: £0.40/min
- Additional: £1.00 every 5 minutes

**Example requests:**
1. Park a small car: { ""vehicleReg"": ""ABC123"", ""vehicleType"": ""Small"" }
2. Park a medium car: { ""vehicleReg"": ""XYZ789"", ""vehicleType"": ""Medium"" }
3. Check status: GET /parking
4. Exit: { ""vehicleReg"": ""ABC123"" }"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Car Park API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CarParkDbContext>();
        await context.Database.MigrateAsync();
        app.Logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while migrating the database");
        throw new InvalidOperationException("Failed to initialize database", ex);
    }
}

await app.RunAsync();

// For testing
public static partial class Program { }
