using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlightManagement.Models;

public partial class Flight
{
    [BindNever]
    public int FlightId { get; set; }

    [Required(ErrorMessage = "Source is required.")]
    [StringLength(100, ErrorMessage = "Source cannot exceed 100 characters.")]
    public string Source { get; set; } = null!;

    [Required(ErrorMessage = "Destination is required.")]
    [StringLength(100, ErrorMessage = "Destination cannot exceed 100 characters.")]
    public string Destination { get; set; } = null!;

    [Required(ErrorMessage = "Arrival time is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time format.")]
    public DateTime Arrival { get; set; }

    [Required(ErrorMessage = "Departure time is required.")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid date and time format.")]
    public DateTime Departure { get; set; }

    [Required(ErrorMessage = "Seats Available is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Seats Available cannot be negative.")]
    public int? SeatsAvailable { get; set; }

    [Required(ErrorMessage = "Total Seats are required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Total Seats must be greater than 0.")]
    public int? TotalSeats { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
    public decimal? Price { get; set; }

    public bool? IsDeleted { get; set; } = false;

    public bool? IsCancelled { get; set; } = false;

    [Required(ErrorMessage = "Flight Name is required.")]
    [StringLength(5, ErrorMessage = "Flight Name cannot exceed 5 characters.")]
    public string FlightName { get; set; } = null!;

    public virtual ICollection<Booking> BookingFlightNameNavigations { get; set; } = new List<Booking>();

    public virtual ICollection<Booking> BookingFlights { get; set; } = new List<Booking>();
}
