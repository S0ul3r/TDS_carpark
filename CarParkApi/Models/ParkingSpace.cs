using CarParkApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkApi.Models;

public class ParkingSpace
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(20)]
    public string? VehicleReg { get; set; }
    
    public VehicleType? VehicleType { get; set; }
    
    [Required]
    public int SpaceNumber { get; set; }
    
    public DateTime? TimeIn { get; set; }
    
    public DateTime? TimeOut { get; set; }
    
    [Required]
    public bool IsOccupied { get; set; }
}

