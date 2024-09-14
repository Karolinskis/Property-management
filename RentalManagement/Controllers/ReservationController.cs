using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RentalManagement.Contexts;
using RentalManagement.Entities;
using RentalManagement.Entities.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Controllers
{
    // FIXME: GET should return only a list of reservations for a specific place

    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationController(AppDbContext context)
        {
            _context = context;
        }

        private async Task<bool> HasConflictingReservations(int placeId, DateTime startDate, DateTime endDate)
        {
            bool isConflicting = await _context.Reservations
                .AnyAsync(r => r.Place.Id == placeId &&
                            r.Status == Status.Approved &&
                            startDate < r.EndDate &&
                            endDate > r.StartDate);

            return isConflicting;
        }

        [HttpGet]
        [Route("place/{placeId}")]
        [SwaggerOperation(Summary = "Gets all Reservations for a specific Place", Description = "Gets all Reservations for a specific Place from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservations were found.", typeof(IEnumerable<Reservation>))]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations(int placeId)
        {
            var reservations = await _context.Reservations
                                    .Where(r => r.Place.Id == placeId)
                                    .ToListAsync();

            var dto = reservations.Select(r =>
            new ReservationDto(
                Id: r.Id,
                PlaceId: r.Place.Id,
                CreatedAt: r.CreatedAt,
                StartDate: r.StartDate,
                EndDate: r.EndDate,
                Status: r.Status,
                Price: r.Price
            ));

            return Ok(dto);
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a Reservation by ID", Description = "Gets a Reservation by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was found.", typeof(Reservation))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new Reservation", Description = "Creates a new Reservation in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The Reservation was created.", typeof(Reservation))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
        {
            // Ensure the Id is not set manually
            reservation.Id = 0;

            var place = await _context.Places.FindAsync(reservation.Place.Id);
            if (place == null)
            {
                return NotFound("Place not found");
            }

            if (await HasConflictingReservations(reservation.Place.Id, reservation.StartDate, reservation.EndDate))
            {
                return BadRequest("The reservation dates overlap with an existing confirmed reservation.");
            }

            if (reservation.Price == 0)
            {
                var days = (reservation.EndDate - reservation.StartDate).Days;
                reservation.Price = days * place.Price;
            }

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a Reservation by ID", Description = "Updates a Reservation by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was updated.", typeof(Reservation))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return BadRequest();
            }

            var existingReservation = await _context.Reservations.FindAsync(id);
            if (existingReservation == null)
            {
                return NotFound("Reservation not found");
            }

            var place = await _context.Places.FindAsync(reservation.Place.Id);
            if (place == null)
            {
                return NotFound("Place not found");
            }

            if (await HasConflictingReservations(reservation.Place.Id, reservation.StartDate, reservation.EndDate))
            {
                return BadRequest("The reservation dates overlap with an existing confirmed reservation.");
            }

            existingReservation.StartDate = reservation.StartDate;
            existingReservation.EndDate = reservation.EndDate;
            existingReservation.Price = reservation.Price;
            existingReservation.Status = reservation.Status;

            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(existingReservation);
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a Reservation by ID", Description = "Deletes a Reservation by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The Reservation was deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var Reservation = await _context.Reservations.FindAsync(id);

            if (Reservation == null)
            {
                return NotFound();
            }

            // FIXME: This will not work if the reservation has a review
            _context.Reservations.Remove(Reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
