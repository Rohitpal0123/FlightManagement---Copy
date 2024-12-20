using BCrypt.Net;
using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagementSystem.Controllers
{
    [Route("signup")]
    [ApiController]
    public class SignupController : Controller
    {
        private readonly FlightManagementContext _context;

        public SignupController(FlightManagementContext context)
        {
            _context = context;
        }

        // GET: Signup
        // [HttpGet("Index")]
        // public IActionResult Index()
        // {
        //     return View();
        // }

        // POST: Signup
        [HttpPost]
        public async Task<IActionResult> Create(string firstName, string lastName, string email, string password, string confirmPassword, string contact, string gender, DateTime? dob)
        {
            try
            {
                if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Password and confirmation password do not match.");
                return Ok(new { message = "Password and confirmation password do not match." });
            }

            if (ModelState.IsValid)
            {
                // Check if the email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "This email is already in use.");
                    return Ok(new { message = "This email is already in use." });
                }

                // Hash the password before saving it to the database
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                // Create a new user
                var user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Gender = gender,
                    Contact = contact,
                    Dob = DateOnly.FromDateTime(dob.Value),
                    Password = hashedPassword,
                    RoleId = 2

                };

                // Add the new user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var emailSender = new GenerateMail();
                System.Console.WriteLine(email);
                string recipientEmail = email;
                System.Console.WriteLine(recipientEmail);
                string subject = "Successful Booking Confirmation !";
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
                    <h2>Sign Up Successful.</h2>
                    <p>Please proceed further with booking flights.</p>
                    <div class='footer'>
                    </div>
                </div>
            </body>
            </html>
        ";
            await emailSender.SendEmailAsync(recipientEmail, subject, htmlBody);

                
                return Ok("Signup Successfull");
            }

            return Ok("Invalid input.");
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new {
                    message = dbEx.Message
                });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }
    }
}
