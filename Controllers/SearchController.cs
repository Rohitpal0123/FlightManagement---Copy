using FlightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlightManagement.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly FlightManagementContext _context;

        public SearchController(FlightManagementContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SearchFlights([FromBody] SearchRequest request)
        {
            try
            {
                var flightsQuery = _context.Flights.AsQueryable();

                // Filter flights based on the provided date or ensure they are in the future
                if (request.Date.HasValue)
                {
                    flightsQuery = flightsQuery.Where(f => f.Departure.Date == request.Date.Value.Date);
                }
                else
                {
                    flightsQuery = flightsQuery.Where(f => f.Departure > DateTime.Now); // Only future flights
                }

                // Filter by source location if provided
                if (!string.IsNullOrEmpty(request.From))
                {
                    flightsQuery = flightsQuery.Where(f => EF.Functions.Like(f.Source.ToLower(), "%" + request.From.ToLower() + "%"));
                }

                // Filter by destination location if provided
                if (!string.IsNullOrEmpty(request.To))
                {
                    flightsQuery = flightsQuery.Where(f => EF.Functions.Like(f.Destination.ToLower(), "%" + request.To.ToLower() + "%"));
                }

                // Execute the query and fetch the results
                var flights = await flightsQuery.ToListAsync();

                // Check if any flights were found
                if (flights.Count == 0)
                {
                    return NotFound(new
                    {
                        message = "No flights found."
                    });
                }

                return Ok(new { flights });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while searching for flights.",
                    error = ex.Message
                });
            }
        }

    }
}
