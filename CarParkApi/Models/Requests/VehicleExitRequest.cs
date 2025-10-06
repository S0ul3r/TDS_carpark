using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CarParkApi.Models.Requests;

public class VehicleExitRequest
{
    [Required]
    [StringLength(20, MinimumLength = 1)]
    [DefaultValue("ABC123")]
    public string VehicleReg { get; set; } = string.Empty;
}