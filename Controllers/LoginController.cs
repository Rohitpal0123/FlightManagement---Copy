using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightManagement.Models;

namespace FlightManagement.Controllers
{

    [Route("Login")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly FlightManagementContext _context;

        public LoginController(FlightManagementContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return Ok(new { message = "Please provide email and password to log in." });
        }

        // POST: Login
        [HttpPost("Index")]
        public async Task<IActionResult> Index(string email, string password)
        {
            // Validate input
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            // Check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user?.IsDeleted == true || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Create user claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
                
                 // Store the user's role
            };

            // Create claims identity
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Persistent cookie for "Remember Me" functionality
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) // Cookie expiration
            };

            // Sign the user in with cookies
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Ok(new { message = $"Login successful! Welcome, {user.FirstName}." });
        }

        // GET: Logout
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign the user out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { message = "You have been logged out successfully." });
        }
    }

    internal class FlightManagementSystemContext
    {
    }
}
