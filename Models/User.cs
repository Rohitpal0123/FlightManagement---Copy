using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlightManagement.Models;

public partial class User
{
    [BindNever]
    public int UserId { get; set; }

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    [CustomValidation(typeof(User), "ValidateDob")]
    public DateOnly Dob { get; set; }

    [Required(ErrorMessage = "Contact is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(15, ErrorMessage = "Contact cannot exceed 15 characters.")]
    public string Contact { get; set; } = null!;

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression("^(M|F|O)$", ErrorMessage = "Gender must be 'M', 'F', or 'O'.")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters.")]
    public string Password { get; set; } = null!;

    public bool? IsDeleted { get; set; } = false;

    public int? RoleId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Role? Role { get; set; }

    public static ValidationResult ValidateDob(DateOnly dob, ValidationContext context)
    {
        if (dob > DateOnly.FromDateTime(DateTime.Now))
        {
            return new ValidationResult("Date of Birth cannot be in the future.");
        }
        return ValidationResult.Success!;
    }
}
