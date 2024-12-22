
using FlightManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace FlightController.Controllers
{

    [Route("booking")]
    [Authorize]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly FlightManagementContext _context;
        public BookingController(FlightManagementContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int? bookingId)
        {
            if (!bookingId.HasValue)
            {
                return BadRequest(new
                {
                    message = "Booking ID cannot be null."
                });
            }

            try
            {
                // Retrieve the userId from the logged-in user's claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated."
                    });
                }

                var userId = int.Parse(userIdClaim.Value);

                // Query the database for the booking related to the logged-in user
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

                if (booking == null)
                {
                    return NotFound(new
                    {
                        message = $"Booking with ID {bookingId} for the logged-in user not found."
                    });
                }

                return Ok(new
                {
                    message = booking
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Retrieve the userId from the logged-in user's claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated."
                    });
                }

                var userId = int.Parse(userIdClaim.Value);

                // Query the database for bookings related to the logged-in user
                var bookings = await _context.Bookings
                    .Where(b => b.UserId == userId)
                    .ToListAsync();

                // Check if any bookings exist for the user
                if (bookings.Count == 0)
                {
                    return NotFound(new
                    {
                        message = "No bookings found for the logged-in user."
                    });
                }

                return Ok(new
                {
                    message = "Bookings retrieved successfully.",
                    bookings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking bookingDetails)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == bookingDetails.FlightId);

            if (bookingDetails == null)
            {
                return BadRequest(new
                {
                    message = "Booking cannot be null."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new
                    {
                        message = "User is not authenticated."
                    });
                }

                var userId = int.Parse(userIdClaim.Value);
                bookingDetails.UserId = userId;

                // Set the BookingDate to the current time
                bookingDetails.BookingDate = DateTime.Now;

                var bookingExists = await _context.Bookings.FirstOrDefaultAsync(
                    b => b.UserId == bookingDetails.UserId && b.FlightName == bookingDetails.FlightName
                );

                if (bookingExists != null)
                {
                    return BadRequest(new
                    {
                        message = $"Booking for userId {bookingDetails.UserId} already exists for the flight {bookingDetails.FlightName}"
                    });
                }

                if (flight.SeatsAvailable < bookingDetails.NumOfPassengers)
                {
                    return Conflict(new
                    {
                        message = "Seats not available."
                    });
                }

                var totalPrice = bookingDetails.NumOfPassengers * flight.Price;
                bookingDetails.TotalPrice = totalPrice;

                await _context.Bookings.AddAsync(bookingDetails);

                flight.SeatsAvailable -= bookingDetails.NumOfPassengers;
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bookingDetails.UserId);
                var userEmail = user.Email;

                var emailSender = new GenerateMail();

                string recipientEmail = userEmail;
                string subject = "Successful Booking Confirmation !";

                // Load the HTML template
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BookingConfirmationTemplate.html");
                string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // Populate the template with dynamic data
                string htmlBody = htmlTemplate
                    .Replace("{{FlightName}}", bookingDetails.FlightName)
                    .Replace("{{SeatNumber}}", bookingDetails.SeatNumber)
                    .Replace("{{NumOfPassengers}}", bookingDetails.NumOfPassengers.ToString())
                    .Replace("{{TotalPrice}}", bookingDetails.TotalPrice.ToString())
                    .Replace("{{BookingDate}}", bookingDetails.BookingDate.ToString());

                await emailSender.SendEmailAsync(recipientEmail, subject, htmlBody);

                return StatusCode(201, new
                {
                    message = $"Please checkout, your total price for Flight {bookingDetails.FlightName} is {bookingDetails.TotalPrice}",
                    CheckoutId = bookingDetails.BookingId
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    message = "Database error occurred.",
                    error = dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        // POST: api/Booking/Cancel
        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            if (bookingId <= 0)
                return BadRequest("Invalid booking ID.");

            try
            {
                Console.WriteLine("BOOKING ID" + bookingId);
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == booking.FlightId);
                Console.WriteLine(flight.FlightId);
                Console.WriteLine("FLIGHT DEPARTURE" + flight.Departure);
                Console.WriteLine("CURRENT DATE" + DateTime.Now);

                if (booking == null)
                    return NotFound("Booking not found.");

                if (booking.Flight == null)
                    return NotFound("Flight details for the booking are missing.");

                if (flight.Departure <= DateTime.Now)
                    return BadRequest("Cannot cancel a booking after the flight has departed.");

                if (booking.IsCancelled ?? false)
                    return BadRequest("Booking has already been cancelled.");

                // Update booking and flight availability
                booking.IsCancelled = true;
                booking.Flight.SeatsAvailable += booking.NumOfPassengers;

                var bookingDetails = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == bookingDetails.UserId);
                var userEmail = user.Email;

                var emailSender = new GenerateMail();

                string recipientEmail = userEmail;
                string subject = "Successful Booking Cancellation !";
                string htmlBody = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px; }}
                    .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); }}
                    table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
                    th, td {{ padding: 10px; text-align: left; border: 1px solid #ddd; }}
                    th {{ background-color: #f2f2f2; }}
                    .footer {{ text-align: center; font-size: 14px; color: #888888; margin-top: 20px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Flight cancelled successfully.</h2>
                    <p>Here are your flight details:</p>
                    <table>
                        <tr>
                            <th>Flight Number</th>
                            <th>SeatNumber</th>
                            <th>Passengers</th>
                            <th>Total Price</th>
                            <th>Booking Date</th>
                        </tr>
                        <tr>
                            <td>{bookingDetails.FlightName}</td>
                            <td>{bookingDetails.SeatNumber}</td>
                            <td>{bookingDetails.NumOfPassengers}</td>
                            <td>{bookingDetails.TotalPrice}</td>
                            <td>{bookingDetails.BookingDate}</td>
                        </tr>
                    </table>
                    <div class='footer'>
                    </div>
                </div>
            </body>
            </html>
        ";


                await emailSender.SendEmailAsync(recipientEmail, subject, htmlBody);

                await _context.SaveChangesAsync();
                return Ok("Booking cancelled successfully.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Database error: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while cancelling the booking: {ex.Message}");
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("getAllBookings")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {

                var bookings = await _context.Bookings
                    .ToListAsync();

                // Check if any bookings exist for the user
                if (bookings.Count == 0)
                {
                    return NotFound(new
                    {
                        message = "No bookings found for the logged-in user."
                    });
                }

                return Ok(new
                {
                    message = "Bookings retrieved successfully.",
                    bookings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }


    }
}

