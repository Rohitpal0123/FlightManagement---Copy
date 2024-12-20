
using FlightManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlightManagement.Controllers
{
  [Authorize(Policy = "AdminPolicy")]
  [Route("flight")]
  [ApiController]
  public class FlightController : ControllerBase
  {
    private readonly FlightManagementContext _context;

    public FlightController(FlightManagementContext context)
    {
      _context = context;
    }

    

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("getAllFlights")]
    public async Task<IActionResult> GetAllFlights()
    {
      var flights = await _context.Flights.ToListAsync();
      return Ok(flights);
    }
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("getActiveFlights")]
    public async Task<IActionResult> GetActiveFlights()
    {
      var flights = await _context.Flights.Where(f => f.IsDeleted == false).ToListAsync();
      return Ok(flights);

    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Flight newFlight)
    {

      if (newFlight == null)
      {
        return BadRequest(new { message = "Flight data cannot be null." });
      }

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      int result = DateTime.Compare(newFlight.Arrival, newFlight.Departure);

      if (result <= 0)
      {
        return BadRequest(new { message = "Arrival datetime cannot be before or same as Departure date time" });
      }


      try
      {
        var flightExist = await _context.Flights.FirstOrDefaultAsync(
          f => f.FlightName == newFlight.FlightName);

        if (flightExist != null)
        {
          var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightName == newFlight.FlightName);
          if (flightExist.IsDeleted == true)
          {
            return BadRequest("Username is already used");

          }

          return Conflict(new
          {

            message = "Flight already exists."
          });
        }

        await _context.Flights.AddAsync(newFlight);

        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
          message = "Flight created successfully.",
          newFlight
        });
      }
      catch (DbUpdateException dbEx)
      {
        return StatusCode(500, new { message = "Database error occured.", error = dbEx.Message });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "An unexpected error occured.", error = ex.Message });
      }
    }



    [Authorize(Policy = "AdminPolicy")]
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
      System.Console.WriteLine("A1");
      var flightExists = await _context.Flights.AnyAsync(f => f.FlightId == id);
      if (!flightExists)
      {
        return BadRequest($"No Flight with Flight id {id} is found");
      }
      var mod = _context.Flights.FirstOrDefault(f => f.FlightId == id);
      System.Console.WriteLine("A2");
      if (mod != null)
      {
        if (mod.IsDeleted == true)
        {
          return BadRequest($" Flight id {id} already cancelleda");
        }
        System.Console.WriteLine("A3");
        mod.IsDeleted = true;
        mod.IsCancelled = true;
        await _context.SaveChangesAsync();


        var mods = await _context.Bookings
                   .Where(f => f.FlightId == mod.FlightId)
                   .ToListAsync();
        foreach (var item in mods)
        {
          item.IsCancelled = true;
          await _context.SaveChangesAsync();
        }

        return Ok($"Flightid {mod.FlightId} is cancelled");



      }
      return BadRequest($"No Flight with Flight id {mod.FlightId} is found");

    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpPut("Update")]
    public async Task<IActionResult> Update(Flight flight)
    {
      var id = flight.FlightId;

      var modify = _context.Flights.FirstOrDefault(f => f.FlightId == id);

      if (modify != null)
      {
        modify.Source = flight.Source;
        modify.Destination = flight.Destination;
        modify.Arrival = flight.Arrival;
        modify.Departure = flight.Departure;
        modify.SeatsAvailable = flight.SeatsAvailable;
        modify.TotalSeats = flight.TotalSeats;
        modify.Price = flight.Price;

        await _context.SaveChangesAsync();

        return Ok($"Flight is updated");
      }

      return NotFound($"Requested Flight id {flight.FlightId} is Not Available in data");


    }




  }
}


