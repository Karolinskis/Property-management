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
    public class PlacesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlacesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a place by ID from the database
        /// </summary>
        /// <returns>A list of all places</returns>
        /// <response code="200">The places were found.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaceDTO>>> GetPlaces()
        {
            var places = await _context.Places.ToListAsync();

            var dto = places.Select(p =>
            {
                return new PlaceDTO(
                    p.Id,
                    p.RoomsCount,
                    p.Size,
                    p.Address,
                    p.Description,
                    p.Price
                );
            });

            return Ok(dto);
        }

        /// <summary>
        /// Gets a place by ID
        /// </summary>
        /// <param name="placeId">ID of the place to get</param>
        /// <returns>The place given in <paramref name="placeId"/></returns>
        /// <response code="200">The place was found.</response>
        /// <response code="404">The place was not found.</response>
        [HttpGet]
        [Route("{placeId}")]
        public async Task<ActionResult<PlaceDTO>> GetPlace(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);

            if (place == null)
            {
                return NotFound();
            }

            var dto = new PlaceDTO(place.Id, place.RoomsCount, place.Size, place.Address, place.Description, place.Price);

            return dto;
        }

        /// <summary>
        /// Creates a new place
        /// </summary>
        /// <param name="createPlaceDto">The place to create</param>
        /// <returns>The created place</returns>
        /// <response code="201">The place was created.</response>
        /// <response code="400">The place is invalid.</response>
        [HttpPost]
        public async Task<ActionResult<PlaceDTO>> CreatePlace([Validate] CreatePlaceDTO createPlaceDto)
        {
            var place = new Place()
            {
                RoomsCount = createPlaceDto.RoomsCount,
                Size = createPlaceDto.Size,
                Address = createPlaceDto.Address,
                Description = createPlaceDto.Description,
                Price = createPlaceDto.Price
            };

            if (place.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            var placeDto = new PlaceDTO
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

        /// <summary>
        /// Updates a place by ID
        /// </summary>
        /// <param name="placeId">ID of the place to update</param>
        /// <param name="updatePlaceDto">Updated information of the place</param>
        /// <returns>An updated place</returns>
        /// <response code="200">The place was updated.</response>
        /// <response code="400">The place is invalid.</response>
        /// <response code="404">The place was not found.</response> 
        [HttpPut]
        [Route("{placeId}")]
        public async Task<ActionResult<PlaceDTO>> UpdatePlace(int placeId, [Validate] UpdatePlaceDTO updatePlaceDto)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);

            if (place is null)
                return NotFound();

            place.RoomsCount = updatePlaceDto.RoomsCount;
            place.Size = updatePlaceDto.Size;
            place.Address = updatePlaceDto.Address;
            place.Description = updatePlaceDto.Description;
            place.Price = updatePlaceDto.Price;

            _context.Places.Update(place);
            await _context.SaveChangesAsync();


            return new PlaceDTO(place.Id, place.RoomsCount, place.Size, place.Address, place.Description, place.Price);
        }

        /// <summary>
        /// Deletes a place by ID
        /// </summary>
        /// <param name="placeId">Place to delete</param>
        /// <response code="204">The place was deleted.</response>
        /// <response code="404">The place was not found.</response>
        [HttpDelete]
        [Route("{placeId}")]
        public async Task<ActionResult<IActionResult>> DeletePlace(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);

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
