using Microsoft.AspNetCore.Authorization;
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
        [SwaggerResponse(200, "The Reservations were found.", typeof(IEnumerable<ReservationDTO>))]
        [SwaggerResponse(404, "The Place was not found.", typeof(ValidationProblemDetails))]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var reservations = await _context.Reservations
                                    .Include(r => r.Place)
                                    .Where(r => r.Place.Id == placeId)
                                    .ToListAsync();

            var dto = reservations.Select(r =>
            {
                return new ReservationDTO(
                    r.Id,
                    r.Place.Id,
                    r.CreatedAt,
                    r.StartDate,
                    r.EndDate,
                    r.Status.ToString(),
                    r.Price
                );
            });

            return Ok(dto);
        }

        /// <summary>
        /// Gets a Reservation by ID
        /// </summary>
        /// <param name="placeId">ID of the place to get reservation from</param>
        /// <param name="reservationId">ID of the reservation</param>
        [SwaggerResponse(200, "The Reservation was found.", typeof(ReservationDTO))]
        [SwaggerResponse(404, "The Place was not found.", typeof(ValidationProblemDetails))]
        [HttpGet]
        [Route("{reservationId}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int placeId, int reservationId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return NotFound("Reservation not found");

            var dto = new ReservationDTO(
                reservation.Id,
                reservation.Place.Id,
                reservation.CreatedAt,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status.ToString(),
                reservation.Price
            );

            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Reservation
        /// </summary>
        /// <param name="placeId">ID of the place to create reservation on</param>
        /// <param name="createReservationDto">Reservation information</param>
        [SwaggerResponse(201, "The Reservation was created.", typeof(ReservationDTO))]
        [SwaggerResponse(400, "The Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(409, "The reservation dates overlap with an existing confirmed reservation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(404, "The Place was not found.", typeof(ValidationProblemDetails))]
        [Authorize(Roles = "Owner, Tennant, Administrator")]
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(int placeId, [Validate] CreateReservationDTO createReservationDto)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);
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
                UserId = "FIXME" // FIXME
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            var reservationDto = new ReservationDTO
            (
                reservation.Id,
                reservation.Place.Id,
                reservation.CreatedAt,
                reservation.StartDate,
                reservation.EndDate,
                reservation.Status.ToString(),
                reservation.Price
            );

            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservationDto);
        }

        /// <summary>
        /// Updates a Reservation by ID
        /// </summary>
        /// <param name="placeId">ID of the place that the reservation is a part of</param>
        /// <param name="reservationId">ID of the reservation to update</param>
        /// <param name="updateReservationDto">Updated reservation information</param>
        [SwaggerResponse(200, "The Reservation was updated.", typeof(ReservationDTO))]
        [SwaggerResponse(400, "The updated Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(404, "The Reservation and/or Place was not found.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(422, "The updated Reservation is invalid", typeof(ValidationProblemDetails))]
        [HttpPut]
        [Authorize(Roles = "Owner, Tennant, Administrator")]
        [Route("{reservationId}")]
        public async Task<ActionResult<ReservationDTO>> UpdateReservation(int placeId, int reservationId, [Validate] UpdateReservationDTO updateReservationDto)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found");

            var existingReservation = await _context.Reservations
                                                    .Include(r => r.Place)
                                                    .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (existingReservation == null)
                return NotFound("Reservation not found");

            if (Utils.HasConflictingReservations(_context, existingReservation.Place.Id, updateReservationDto.StartDate.ToUniversalTime(), updateReservationDto.EndDate.ToUniversalTime()))
                return Conflict("The reservation dates overlap with an existing confirmed reservation.");

            existingReservation.StartDate = updateReservationDto.StartDate;
            existingReservation.EndDate = updateReservationDto.EndDate;
            existingReservation.Price = updateReservationDto.Price;
            if (!Enum.TryParse<Status>(updateReservationDto.Status, out Status statusResult))
                return UnprocessableEntity("Invalid status");
            existingReservation.Status = statusResult;

            _context.Entry(existingReservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var dto = new ReservationDTO(
                existingReservation.Id,
                existingReservation.Place.Id,
                existingReservation.CreatedAt,
                existingReservation.StartDate,
                existingReservation.EndDate,
                existingReservation.Status.ToString(),
                existingReservation.Price
            );

            return Ok(dto);
        }

        /// <summary>
        /// Deletes a Reservation by its ID
        /// </summary>
        /// <param name="placeId">ID of the place that the reservation is a part of</param>
        /// <param name="reservationId">ID of the reservation</param>
        [SwaggerResponse(204, "The Reservation was deleted.")]
        [SwaggerResponse(400, "The Reservation does not belong to the specified place.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(404, "The Reservation and/or the place was not found.", typeof(ValidationProblemDetails))]
        [HttpDelete]
        [Authorize(Roles = "Owner, Tennant, Administrator")]
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

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
