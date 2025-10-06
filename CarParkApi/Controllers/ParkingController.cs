using CarParkApi.Models.Requests;
using CarParkApi.Models.Responses;
using CarParkApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarParkApi.Controllers;

[ApiController]
[Route("parking")]
public class ParkingController : ControllerBase
{
    private readonly IParkingService _parkingService;
    private readonly ILogger<ParkingController> _logger;

    public ParkingController(IParkingService parkingService, ILogger<ParkingController> logger)
    {
        _parkingService = parkingService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ParkingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ParkVehicle([FromBody] ParkVehicleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.VehicleReg))
            {
                return BadRequest("Vehicle registration is required.");
            }

            if (string.IsNullOrWhiteSpace(request.VehicleType))
            {
                return BadRequest("Vehicle type is required.");
            }

            var response = await _parkingService.ParkVehicleAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("full"))
        {
            _logger.LogWarning(ex, "Car park full");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parking vehicle");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while parking the vehicle.");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(SpaceStatusResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpaceStatus()
    {
        try
        {
            var response = await _parkingService.GetSpaceStatusAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting space status");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving space status.");
        }
    }

    [HttpPost("exit")]
    [ProducesResponseType(typeof(ExitResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ProcessVehicleExit([FromBody] VehicleExitRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.VehicleReg))
            {
                return BadRequest("Vehicle registration is required.");
            }

            var response = await _parkingService.ProcessVehicleExitAsync(request);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing exit");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing vehicle exit.");
        }
    }
}

