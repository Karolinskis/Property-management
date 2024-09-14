using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using RentalManagement.Contexts;
using RentalManagement.Entities;
using RentalManagement.Entities.DTOs;
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
        [SwaggerOperation(Summary = "Gets all places", Description = "Gets all places from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The places were found.", typeof(IEnumerable<Place>))]
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            return await _context.Places.ToListAsync();
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a place by ID", Description = "Gets a place by ID from the database.")]
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
        [SwaggerOperation(Summary = "Creates a new place", Description = "Creates a new place in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The place was created.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<PlaceDto>> CreatePlace([Validate] CreatePlaceDto createPlaceDto)
        {
            var place = new Place()
            {
                RoomsCount = createPlaceDto.RoomsCount,
                Size = createPlaceDto.Size,
                Address = createPlaceDto.Address,
                Description = createPlaceDto.Description,
                Price = createPlaceDto.Price
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            var placeDto = new PlaceDto
            (
                place.Id,
                place.RoomsCount,
                place.Size,
                place.Address,
                place.Description,
                place.Price
            );

            return CreatedAtAction(nameof(GetPlace), new { id = place.Id }, placeDto);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a place by ID", Description = "Updates a place by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The place was updated.", typeof(Place))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<PlaceDto>> UpdatePlace(int id, [Validate] UpdatePlaceDto updatePlaceDto)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == id);

            if (place is null)
            {
                return NotFound();
            }

            place.RoomsCount = updatePlaceDto.RoomsCount;
            place.Size = updatePlaceDto.Size;
            place.Address = updatePlaceDto.Address;
            place.Description = updatePlaceDto.Description;
            place.Price = updatePlaceDto.Price;

            _context.Places.Update(place);
            await _context.SaveChangesAsync();


            return new PlaceDto(place.Id, place.RoomsCount, place.Size, place.Address, place.Description, place.Price);
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a place by ID", Description = "Deletes a place by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The place was deleted.")]
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

            return NoContent();
        }
    }
}
