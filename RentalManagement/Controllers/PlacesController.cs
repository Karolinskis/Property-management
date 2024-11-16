using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        /// Get all places
        /// </summary>
        [SwaggerResponse(StatusCodes.Status200OK, "The places were found.", typeof(IEnumerable<PlaceDTO>))]
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
        /// Gets a place by its ID
        /// </summary>
        /// <param name="placeId">ID of the place to get</param>
        [SwaggerResponse(StatusCodes.Status200OK, "The place was found.", typeof(PlaceDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
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
        [SwaggerResponse(StatusCodes.Status201Created, "The place was created.", typeof(PlaceDTO))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to create a place.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        [HttpPost]
        [Authorize(Roles = UserRoles.Owner)]
        public async Task<ActionResult<PlaceDTO>> CreatePlace([Validate] CreatePlaceDTO createPlaceDto)
        {
            var place = new Place()
            {
                RoomsCount = createPlaceDto.RoomsCount,
                Size = createPlaceDto.Size,
                Address = createPlaceDto.Address,
                Description = createPlaceDto.Description,
                Price = createPlaceDto.Price,
                UserId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
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

            return CreatedAtAction(nameof(GetPlace), new { placeId = place.Id }, placeDto);
        }

        /// <summary>
        /// Updates a place by ID
        /// </summary>
        /// <param name="placeId">ID of the place to update</param>
        /// <param name="updatePlaceDto">Updated information of the place</param>
        [SwaggerResponse(StatusCodes.Status200OK, "The place was updated.", typeof(PlaceDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The place is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to update this place.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        [HttpPut]
        [Authorize(Roles = UserRoles.Owner)]
        [Route("{placeId}")]
        public async Task<ActionResult<PlaceDTO>> UpdatePlace(int placeId, [Validate] UpdatePlaceDTO updatePlaceDto)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);

            if (place is null)
                return NotFound("Place not found.");

            if (!HttpContext.User.IsInRole(UserRoles.Admin) &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != place.UserId)
            {
                return Forbid();
            }

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
        [SwaggerResponse(StatusCodes.Status204NoContent, "The place was deleted.")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to delete this place.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The place was not found.", typeof(ValidationProblemDetails))]
        [HttpDelete]
        [Authorize(Roles = UserRoles.Owner)]
        [Route("{placeId}")]
        public async Task<ActionResult> DeletePlace(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);

            if (place == null)
            {
                return NotFound("Place not found.");
            }

            if (!HttpContext.User.IsInRole(UserRoles.Admin) &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != place.UserId)
            {
                return Forbid();
            }

            _context.Places.Remove(place);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
