using Microsoft.AspNetCore.Mvc;
using FlightManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace FlightManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("checkout")]
    public class CheckoutController : ControllerBase
    {
        private readonly FlightManagementContext _context;

        public CheckoutController(FlightManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] CheckOutRequest id)
        {
            try
            {
                var booking_id = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id.BookingId);
                if (booking_id == null)
                {
                    return NotFound("Booking not found");
                }

                if ((bool)booking_id.IsCheckedOut)
                {
                    return Ok("Booking is already checked out");
                }

                booking_id.IsCheckedOut = true;
                await _context.SaveChangesAsync();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == booking_id.UserId);
                var userEmail = user.Email;

                var emailSender = new GenerateMail();

                string recipientEmail = userEmail;
                string subject = "Checkout Successful!";
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
                        <h2>Checkout Successful.</h2>
                        <p>Happy Journey.</p>
                        <div class='footer'>
                        </div>
                    </div>
                </body>
                </html>
                ";
                 await emailSender.SendEmailAsync(recipientEmail, subject, htmlBody);
                return Ok("Flight checked out successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }

    public class CheckOutRequest
    {
        [Required(ErrorMessage = "Booking ID is required.")]
        public int BookingId { get; set; }
    }
}
