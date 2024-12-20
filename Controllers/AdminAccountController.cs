using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlightManagement.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;



[Route("AdminAccount")]
[ApiController]
public class AdminAccountController : Controller
{
  private readonly FlightManagementContext _context;

  public AdminAccountController(FlightManagementContext context)
  {
    _context = context;
  }


  [HttpPost("AdminLogin")]
  public async Task<IActionResult> AdminLogin([FromBody] Admin credentials)
  {
    var email = credentials.email;
    var password = credentials.password;

    var user = await _context.Users.FirstOrDefaultAsync(f => f.Email == email && f.RoleId == 1);

    var role = await _context.Roles.FirstOrDefaultAsync(f => f.RoleId == user.RoleId);


    if (email == null || password == null)
    { ModelState.AddModelError("", "Invalid login attempt."); }

    if (email ==user.Email && password == user.Password)
    {

      var claims = new List<Claim>
            {
                new(ClaimTypes.Name, email),
                new(ClaimTypes.Role, role.RoleName )
            };

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      var principal = new ClaimsPrincipal(identity);

      await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

      System.Console.WriteLine("A2");


      return Ok("Login Successful");

    }



    System.Console.WriteLine("A3");
    return BadRequest("invalid input");
  }

  [HttpGet("Logout")]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return Ok("You Logged Out\n Need to log in before use");
  }
}