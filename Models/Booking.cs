using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace FlightManagement.Models;

public partial class Booking
{
    [BindNever]
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Flight ID is required.")]
    public int FlightId { get; set; }
    public int UserId { get; set; }

    [Required(ErrorMessage = "Seat Number is required.")]
    [StringLength(10, ErrorMessage = "Seat Number cannot exceed 10 characters.")]
    public string SeatNumber { get; set; } = null!;

    [Required(ErrorMessage = "Booking Date is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time format.")]
    public DateTime? BookingDate { get; set; }

    public bool? IsDeleted { get; set; } = false;

    [Required(ErrorMessage = "Number of Passengers is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of Passengers must be at least 1.")]
    public int? NumOfPassengers { get; set; }

    public bool? IsCancelled { get; set; } = false;

    public decimal? TotalPrice {get; set;}

    public bool? IsCheckedOut {get; set;} = false;

    [StringLength(5, ErrorMessage = "Flight Name cannot exceed 5 characters.")]
    public string? FlightName { get; set; }

    [JsonIgnore]
    [ValidateNever] 
    public virtual Flight? Flight { get; set; }

    [JsonIgnore]
    [ValidateNever] 
    public virtual Flight? FlightNameNavigation { get; set; }

    [JsonIgnore]
    [ValidateNever] 
    public virtual User? User { get; set; }
}
