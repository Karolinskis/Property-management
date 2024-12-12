using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using RentalManagement.Auth;
using RentalManagement.Contexts;
using RentalManagement.Entities;
using RentalManagement.Entities.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Controllers
{
    [ApiController]
    [Route("api/Places/{placeId}/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all Reservations for a specific Place
        /// </summary>
        /// <param name="placeId">ID of the place to get reservations from</param>
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservations were found.", typeof(IEnumerable<ReservationDTO>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place was not found.", typeof(ValidationProblemDetails))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var reservations = await _context.Reservations
                                    .Include(r => r.Place)
                                    .Include(r => r.User)
                                    .Where(r => r.Place.Id == placeId)
                                    .ToListAsync();

            var dto = reservations.Select(r =>
            {
                var placeDto = new PlaceDTO(
                    r.Place.Id,
                    r.Place.RoomsCount,
                    r.Place.Size,
                    r.Place.Address,
                    r.Place.Description,
                    r.Place.Price,
                    r.Place.UserId
                );

                var userDto = new UserDTO(
                    r.User.Id,
                    r.User.UserName ?? string.Empty,
                    r.User.Email ?? string.Empty
                );

                return new ReservationDTO(
                    r.Id,
                    r.Place.Id,
                    r.CreatedAt,
                    r.StartDate,
                    r.EndDate,
                    r.Status.ToString(),
                    r.Price,
                    placeDto,
                    userDto
                );
            });

            return Ok(dto);
        }

        /// <summary>
        /// Gets a Reservation by ID
        /// </summary>
        /// <param name="placeId">ID of the place to get reservation from</param>
        /// <param name="reservationId">ID of the reservation</param>
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was found.", typeof(ReservationDTO))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place or Reservation was not found.", typeof(ValidationProblemDetails))]
        [HttpGet]
        [Route("{reservationId}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int placeId, int reservationId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .Include(r => r.User)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return NotFound("Reservation not found");

            var placeDto = new PlaceDTO(
                reservation.Place.Id,
                reservation.Place.RoomsCount,
                reservation.Place.Size,
                reservation.Place.Address,
                reservation.Place.Description,
                reservation.Place.Price,
                reservation.Place.UserId
            );

            var userDto = new UserDTO(
                reservation.User.Id,
                reservation.User.UserName ?? string.Empty,
                reservation.User.Email ?? string.Empty
            );

            var dto = new ReservationDTO(
                reservation.Id,
                reservation.Place.Id,
                reservation.CreatedAt,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status.ToString(),
                reservation.Price,
                placeDto,
                userDto
            );

            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Reservation
        /// </summary>
        /// <param name="placeId">ID of the place to create reservation on</param>
        /// <param name="createReservationDto">Reservation information</param>
        [SwaggerResponse(StatusCodes.Status201Created, "The Reservation was created.", typeof(ReservationDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to create a reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place was not found.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "The reservation dates overlap with an existing confirmed reservation.", typeof(ValidationProblemDetails))]
        [Authorize(Roles = UserRoles.Owner + "," + UserRoles.Tenant)]
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(int placeId, [Validate] CreateReservationDTO createReservationDto)
        {
            var place = await _context.Places
                                    .Include(r => r.User)
                                    .FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null)
                return NotFound("Place not found");

            if (Utils.HasConflictingReservations(_context, placeId, createReservationDto.StartDate.ToUniversalTime(), createReservationDto.EndDate.ToUniversalTime()))
                return Conflict("The reservation dates overlap with an existing confirmed reservation.");

            // If the price is not set otherwise, calculate it based on the number of days
            float reservationPrice = 0;
            if (createReservationDto.Price == 0)
            {
                var days = (createReservationDto.EndDate - createReservationDto.StartDate).Days;
                reservationPrice = days * place.Price;
            }

            var reservation = new Reservation
            {
                Place = place,
                CreatedAt = DateTime.UtcNow,
                StartDate = createReservationDto.StartDate,
                EndDate = createReservationDto.EndDate,
                Status = Status.Pending,
                Price = reservationPrice != 0 ? reservationPrice : createReservationDto.Price,
                UserId = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var placeDto = new PlaceDTO(
                reservation.Place.Id,
                reservation.Place.RoomsCount,
                reservation.Place.Size,
                reservation.Place.Address,
                reservation.Place.Description,
                reservation.Place.Price,
                reservation.Place.UserId
            );

            var user = await _context.Users.FindAsync(reservation.UserId);
            if (user == null)
                return NotFound("User not found");

            var userDto = new UserDTO(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty
            );

            var reservationDto = new ReservationDTO(
                reservation.Id,
                reservation.Place.Id,
                reservation.CreatedAt,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status.ToString(),
                reservation.Price,
                placeDto,
                userDto
            );

            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservationDto);
        }

        /// <summary>
        /// Updates a Reservation by ID
        /// </summary>
        /// <param name="placeId">ID of the place that the reservation is a part of</param>
        /// <param name="reservationId">ID of the reservation to update</param>
        /// <param name="updateReservationDto">Updated reservation information</param>
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was updated.", typeof(ReservationDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The updated Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to update this reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to update this reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation and/or Place was not found.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "The updated Reservation is invalid", typeof(ValidationProblemDetails))]
        [Authorize(Roles = UserRoles.Owner + "," + UserRoles.Tenant)]
        [HttpPut]
        [Route("{reservationId}")]
        public async Task<ActionResult<ReservationDTO>> UpdateReservation(int placeId, int reservationId, [Validate] UpdateReservationDTO updateReservationDto)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var existingReservation = await _context.Reservations
                                                    .Include(r => r.Place)
                                                    .Include(r => r.User)
                                                    .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (existingReservation == null)
                return NotFound("Reservation not found");

            if (!HttpContext.User.IsInRole(UserRoles.Admin) &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != existingReservation.UserId &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != place.UserId)
            {
                return Forbid();
            }

            existingReservation.StartDate = updateReservationDto.StartDate;
            existingReservation.EndDate = updateReservationDto.EndDate;
            existingReservation.Price = updateReservationDto.Price;
            if (!Enum.TryParse<Status>(updateReservationDto.Status, out Status statusResult))
                return UnprocessableEntity("Invalid status");
            existingReservation.Status = statusResult;

            _context.Entry(existingReservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var placeDto = new PlaceDTO(
                existingReservation.Place.Id,
                existingReservation.Place.RoomsCount,
                existingReservation.Place.Size,
                existingReservation.Place.Address,
                existingReservation.Place.Description,
                existingReservation.Place.Price,
                existingReservation.Place.UserId
            );

            var userDto = new UserDTO(
                existingReservation.User.Id,
                existingReservation.User.UserName ?? string.Empty,
                existingReservation.User.Email ?? string.Empty
            );

            var dto = new ReservationDTO(
                existingReservation.Id,
                existingReservation.Place.Id,
                existingReservation.CreatedAt,
                existingReservation.StartDate,
                existingReservation.EndDate,
                existingReservation.Status.ToString(),
                existingReservation.Price,
                placeDto,
                userDto
            );

            return Ok(dto);
        }

        /// <summary>
        /// Deletes a Reservation by its ID
        /// </summary>
        /// <param name="placeId">ID of the place that the reservation is a part of</param>
        /// <param name="reservationId">ID of the reservation</param>
        [SwaggerResponse(StatusCodes.Status204NoContent, "The Reservation was deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Reservation does not belong to the specified place.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "You are not authorized to delete this reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "You are not allowed to delete this reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation and/or the place was not found.", typeof(ValidationProblemDetails))]
        [Authorize(Roles = UserRoles.Owner + "," + UserRoles.Tenant)]
        [HttpDelete]
        [Route("{reservationId}")]
        public async Task<ActionResult> DeleteReservation(int placeId, int reservationId)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null)
                return NotFound("Place not found");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return NotFound("Reservation not found");

            if (reservation.Place.Id != placeId)
                return BadRequest("Reservation does not belong to the specified place.");

            if (!HttpContext.User.IsInRole(UserRoles.Admin) &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != reservation.UserId &&
                HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != place.UserId)
            {
                return Forbid();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
