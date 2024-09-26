using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
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

        // TODO: Return status not as a number but as a string

        [HttpGet]
        [Route("place/{placeId}")]
        [SwaggerOperation(Summary = "Gets all Reservations for a specific Place", Description = "Gets all Reservations for a specific Place from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservations were found.", typeof(IEnumerable<ReservationDto>))]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations(int placeId)
        {
            var reservations = await _context.Reservations
                                    .Include(r => r.Place)
                                    .Where(r => r.Place.Id == placeId)
                                    .ToListAsync();

            var dto = reservations.Select(r =>
            {
                return new ReservationDto(
                    Id: r.Id,
                    PlaceId: r.Place.Id,
                    CreatedAt: r.CreatedAt,
                    StartDate: r.StartDate,
                    EndDate: r.EndDate,
                    Status: r.Status.ToString(),
                    Price: r.Price
                );
            });

            return Ok(dto);
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a Reservation by ID", Description = "Gets a Reservation by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was found.", typeof(ReservationDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            var dto = new ReservationDto(
                Id: reservation.Id,
                PlaceId: reservation.Place.Id,
                CreatedAt: reservation.CreatedAt,
                StartDate: reservation.StartDate,
                EndDate: reservation.EndDate,
                Status: reservation.Status.ToString(),
                Price: reservation.Price
            );

            return Ok(dto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new Reservation", Description = "Creates a new Reservation in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The Reservation was created.", typeof(ReservationDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Reservation is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReservationDto>> CreateReservation([Validate] CreateReservationDto createReservationDto)
        {
            var place = await _context.Places.FindAsync(createReservationDto.PlaceId);
            if (place == null)
            {
                return NotFound("Place not found");
            }

            if (await HasConflictingReservations(createReservationDto.PlaceId, createReservationDto.StartDate, createReservationDto.EndDate))
            {
                return BadRequest("The reservation dates overlap with an existing confirmed reservation.");
            }

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
                Price = reservationPrice != 0 ? reservationPrice : createReservationDto.Price
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a Reservation by ID", Description = "Updates a Reservation by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reservation was updated.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReservationDto>> UpdateReservation(int id, UpdateReservationDto updateReservationDto)
        {
            var existingReservation = await _context.Reservations
                                                    .Include(r => r.Place)
                                                    .FirstOrDefaultAsync(r => r.Id == id);
            if (existingReservation == null)
            {
                return NotFound("Reservation not found");
            }

            if (await HasConflictingReservations(existingReservation.Place.Id, updateReservationDto.StartDate, updateReservationDto.EndDate))
            {
                return BadRequest("The reservation dates overlap with an existing confirmed reservation.");
            }

            existingReservation.StartDate = updateReservationDto.StartDate;
            existingReservation.EndDate = updateReservationDto.EndDate;
            existingReservation.Price = updateReservationDto.Price;
            existingReservation.Status = Enum.Parse<Status>(updateReservationDto.Status);

            _context.Entry(existingReservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var dto = new ReservationDto(
                Id: existingReservation.Id,
                PlaceId: existingReservation.Place.Id,
                CreatedAt: existingReservation.CreatedAt,
                StartDate: existingReservation.StartDate,
                EndDate: existingReservation.EndDate,
                Status: existingReservation.Status.ToString(),
                Price: existingReservation.Price
            );

            return Ok(dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a Reservation by ID", Description = "Deletes a Reservation by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The Reservation was deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Reservation was not found.", typeof(ValidationProblemDetails))]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var Reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == id);

            if (Reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(Reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
