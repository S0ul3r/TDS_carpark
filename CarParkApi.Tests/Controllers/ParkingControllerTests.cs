using CarParkApi.Controllers;
using CarParkApi.Models.Requests;
using CarParkApi.Models.Responses;
using CarParkApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CarParkApi.Tests.Controllers;

public class ParkingControllerTests
{
    private readonly Mock<IParkingService> _mockService;
    private readonly Mock<ILogger<ParkingController>> _mockLogger;
    private readonly ParkingController _controller;

    public ParkingControllerTests()
    {
        _mockService = new Mock<IParkingService>();
        _mockLogger = new Mock<ILogger<ParkingController>>();
        _controller = new ParkingController(_mockService.Object, _mockLogger.Object);
    }

    #region ParkVehicle Tests

    [Fact]
    public async Task ParkVehicle_ValidRequest_ReturnsOkWithParkingResponse()
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = "ABC123",
            VehicleType = "Medium"
        };

        var expectedResponse = new ParkingResponse
        {
            VehicleReg = "ABC123",
            SpaceNumber = 5,
            TimeIn = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ParkVehicleAsync(request))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.ParkVehicle(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ParkingResponse>(okResult.Value);
        Assert.Equal("ABC123", response.VehicleReg);
        Assert.Equal(5, response.SpaceNumber);
    }

    [Theory]
    [InlineData("", "Medium", "Vehicle registration is required.")]
    [InlineData(null, "Medium", "Vehicle registration is required.")]
    [InlineData("ABC123", "", "Vehicle type is required.")]
    [InlineData("ABC123", null, "Vehicle type is required.")]
    public async Task ParkVehicle_InvalidRequest_ReturnsBadRequest(
        string vehicleReg, string vehicleType, string expectedMessage)
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = vehicleReg!,
            VehicleType = vehicleType!
        };

        var result = await _controller.ParkVehicle(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(expectedMessage, badRequestResult.Value);
    }

    [Fact]
    public async Task ParkVehicle_InvalidVehicleType_ReturnsBadRequest()
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = "ABC123",
            VehicleType = "ExtraLarge"
        };

        _mockService.Setup(s => s.ParkVehicleAsync(request))
            .ThrowsAsync(new ArgumentException("Invalid vehicle type"));

        var result = await _controller.ParkVehicle(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("Invalid vehicle type", badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task ParkVehicle_CarParkFull_ReturnsServiceUnavailable()
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = "ABC123",
            VehicleType = "Medium"
        };

        _mockService.Setup(s => s.ParkVehicleAsync(request))
            .ThrowsAsync(new InvalidOperationException("Car park is full"));

        var result = await _controller.ParkVehicle(request);

        var serviceUnavailableResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, serviceUnavailableResult.StatusCode);
    }

    [Fact]
    public async Task ParkVehicle_VehicleAlreadyParked_ReturnsBadRequest()
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = "ABC123",
            VehicleType = "Medium"
        };

        _mockService.Setup(s => s.ParkVehicleAsync(request))
            .ThrowsAsync(new InvalidOperationException("Vehicle is already parked"));

        var result = await _controller.ParkVehicle(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("already parked", badRequestResult.Value!.ToString());
    }

    [Fact]
    public async Task ParkVehicle_UnexpectedException_ReturnsInternalServerError()
    {
        var request = new ParkVehicleRequest
        {
            VehicleReg = "ABC123",
            VehicleType = "Medium"
        };

        _mockService.Setup(s => s.ParkVehicleAsync(request))
            .ThrowsAsync(new Exception("Database connection failed"));

        var result = await _controller.ParkVehicle(request);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
    }

    #endregion

    #region GetSpaceStatus Tests

    [Fact]
    public async Task GetSpaceStatus_ReturnsOkWithSpaceStatus()
    {
        var expectedResponse = new SpaceStatusResponse
        {
            AvailableSpaces = 15,
            OccupiedSpaces = 5
        };

        _mockService.Setup(s => s.GetSpaceStatusAsync())
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetSpaceStatus();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SpaceStatusResponse>(okResult.Value);
        Assert.Equal(15, response.AvailableSpaces);
        Assert.Equal(5, response.OccupiedSpaces);
    }

    [Fact]
    public async Task GetSpaceStatus_ServiceThrowsException_ReturnsInternalServerError()
    {
        _mockService.Setup(s => s.GetSpaceStatusAsync())
            .ThrowsAsync(new Exception("Database error"));

        var result = await _controller.GetSpaceStatus();

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
    }

    #endregion

    #region ProcessVehicleExit Tests

    [Fact]
    public async Task ProcessVehicleExit_ValidRequest_ReturnsOkWithExitResponse()
    {
        var request = new VehicleExitRequest { VehicleReg = "ABC123" };
        
        var expectedResponse = new ExitResponse
        {
            VehicleReg = "ABC123",
            VehicleCharge = 4.40,
            TimeIn = DateTime.UtcNow.AddMinutes(-12),
            TimeOut = DateTime.UtcNow
        };

        _mockService.Setup(s => s.ProcessVehicleExitAsync(request))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.ProcessVehicleExit(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ExitResponse>(okResult.Value);
        Assert.Equal("ABC123", response.VehicleReg);
        Assert.Equal(4.40, response.VehicleCharge);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task ProcessVehicleExit_EmptyVehicleReg_ReturnsBadRequest(string? vehicleReg)
    {
        var request = new VehicleExitRequest { VehicleReg = vehicleReg ?? string.Empty };

        var result = await _controller.ProcessVehicleExit(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Vehicle registration is required.", badRequestResult.Value);
    }

    [Fact]
    public async Task ProcessVehicleExit_VehicleNotFound_ReturnsNotFound()
    {
        var request = new VehicleExitRequest { VehicleReg = "NOTFOUND" };

        _mockService.Setup(s => s.ProcessVehicleExitAsync(request))
            .ThrowsAsync(new KeyNotFoundException("Vehicle not found"));

        var result = await _controller.ProcessVehicleExit(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("not found", notFoundResult.Value!.ToString()!);
    }

    [Fact]
    public async Task ProcessVehicleExit_UnexpectedException_ReturnsInternalServerError()
    {
        var request = new VehicleExitRequest { VehicleReg = "ABC123" };

        _mockService.Setup(s => s.ProcessVehicleExitAsync(request))
            .ThrowsAsync(new Exception("Database connection failed"));

        var result = await _controller.ProcessVehicleExit(request);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
    }

    #endregion
}

