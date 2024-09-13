using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RentalManagement.Contexts;
using RentalManagement.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlaceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Gets all places.", Description = "Gets all places from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The places were found.", typeof(IEnumerable<Place>))]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            return await _context.Places.ToListAsync();
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a place by ID.", Description = "Gets a place by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The place was found.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Place>> GetPlace(int id)
        {
            var place = await _context.Places.FindAsync(id);

            if (place == null)
            {
                return NotFound();
            }

            return place;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new place.", Description = "Creates a new place in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The place was created.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Place>> CreatePlace(Place place)
        {
            await _context.Places.AddAsync(place);
            await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(CreatePlace), new { id = place.Id }, place);
            return CreatedAtAction(nameof(CreatePlace), place);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a place by ID.", Description = "Updates a place by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The place was updated.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Place>> UpdatePlace(int id, Place place)
        {
            if (id != place.Id)
            {
                return BadRequest(new ValidationProblemDetails { Title = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPlace = await _context.Places.FindAsync(id);
            if (existingPlace == null)
            {
                return NotFound();
            }

            _context.Entry(existingPlace).CurrentValues.SetValues(place);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return place;
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a place by ID.", Description = "Deletes a place by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The place was deleted.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Place>> DeletePlace(int id)
        {
            var place = await _context.Places.FindAsync(id);

            if (place == null)
            {
                return NotFound();
            }

            _context.Places.Remove(place);
            await _context.SaveChangesAsync();

            return place;
        }
    }
}
