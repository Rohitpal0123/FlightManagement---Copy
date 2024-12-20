using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using FlightManagement.lib;
using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightManagement.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly FlightManagementContext _context;

        public UserController(FlightManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest(new { message = "User cannot be null." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userExist = await _context.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
                if (userExist != null)
                {
                    return Conflict(new {
                        message = "User already exists.",
                        newUser
                    });
                } 

                var contactExists = await _context.Users.FirstOrDefaultAsync(u => u.Contact == newUser.Contact);
                if (contactExists != null)
                {
                    return Conflict(new {
                        message = "Contact already exists."
                    });
                } 

                newUser.Password = PasswordHasher.HashPassword(newUser.Password);

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                
                

                return StatusCode(201, new {
                    message = "User created successfully."
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { message = "Database error occured", error = dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occured", error = ex.Message });

            }
        }
    }
}