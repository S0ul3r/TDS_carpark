using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CarParkApi.Models.Requests;

public class ParkVehicleRequest
{
    [Required]
    [StringLength(20, MinimumLength = 1)]
    [DefaultValue("ABC123")]
    public string VehicleReg { get; set; } = string.Empty;
    
    [Required]
    [DefaultValue("Medium")]
    public string VehicleType { get; set; } = string.Empty;
}