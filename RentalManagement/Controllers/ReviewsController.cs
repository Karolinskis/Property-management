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
    [Route("api/Places/{placeId}")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all Reviews for a specific Place
        /// </summary>
        /// <param name="placeId">ID of the place</param>
        /// <returns>A list of Reviews for the given <paramref name="placeId"/></returns>
        /// <response code="200">The Reviews were found.</response>
        /// <response code="404">The Place was not found.</response> 
        [HttpGet]
        [Route("[controller]")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found.");

            var reviews = await _context.Reviews
                                        .Where(r => _context.Reservations
                                                            .Any(res => res.Place.Id == placeId))
                                        .ToListAsync();

            var reviewDtos = reviews.Select(r => new ReviewDTO(
                r.Id,
                r.ReservationId,
                r.Rating,
                r.Comment
            )).ToList();

            return Ok(reviewDtos);
        }

        /// <summary>
        /// Gets a Review by the Reservation ID
        /// </summary>
        /// <param name="placeId">ID of the reservation's place</param>
        /// <param name="reservationId">ID of the reservation</param>
        /// <returns>The specific reservation</returns>
        /// <response code="200">The Review was found.</response>
        /// <response code="404">The Review or Place was not found.</response> 
        [HttpGet]
        [Route("Reservations/{reservationId}/[controller]")]
        public async Task<ActionResult<ReviewDTO>> GetReviewByReservationId(int placeId, int reservationId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found.");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                return NotFound("Reservation not found or does not belong to the specified place.");

            var review = await _context.Reviews
                                    .Where(r => r.ReservationId == reservationId)
                                    .FirstOrDefaultAsync();

            if (review == null)
                return NotFound("Review not found.");

            var dto = new ReviewDTO(
                review.Id,
                review.ReservationId,
                review.Rating,
                review.Comment
            );

            return Ok(dto);
        }

        /// <summary>
        /// Creates a new Review
        /// </summary>
        /// <param name="placeId">ID of the place that the reservation was made in</param>
        /// <param name="reservationId">ID of the reservation</param>
        /// <param name="createReviewDto">Review details</param>
        /// <returns>The created review</returns>
        /// <response code="201">The Review was created.</response>
        /// <response code="400">A review for this reservation already exists.</response>
        /// <response code="404">The Place or Reservation was not found.</response> 
        [HttpPost]
        [Route("Reservations/{reservationId}/[controller]")]
        public async Task<ActionResult<ReviewDTO>> CreateReview(int placeId, int reservationId, [Validate] CreateReviewDTO createReviewDto)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found.");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                return NotFound("Reservation not found or does not belong to the specified place.");

            var review = new Review
            {
                ReservationId = reservationId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment
            };

            var existingReview = await _context.Reviews
                                        .AnyAsync(r => r.ReservationId == review.ReservationId);

            if (existingReview)
                return BadRequest("A review for this reservation already exists.");

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateReview), new { id = review.Id }, review);
        }

        /// <summary>
        /// Updates a Review by ID
        /// </summary>
        /// <param name="placeId">ID of the review's place</param>
        /// <param name="reservationId">ID of the review's reservation</param>
        /// <param name="reviewId">ID of the review</param>
        /// <param name="updateReviewDto">Updated review information</param>
        /// <returns>Updated review information</returns>
        /// <response code="200">The Review was updated.</response>
        /// <response code="400">The Review is invalid.</response>
        /// <response code="404">The Review was not found.</response> 
        [HttpPut]
        [Route("Reservations/{reservationId}/[controller]/{reviewId}")]
        public async Task<ActionResult<ReviewDTO>> UpdateReview(int placeId, int reservationId, int reviewId, [Validate] UpdateReviewDTO updateReviewDto)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found.");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                return NotFound("Reservation not found or does not belong to the specified place.");

            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;

            await _context.SaveChangesAsync();

            return new ReviewDTO(
                review.Id,
                review.ReservationId,
                review.Rating,
                review.Comment
            );

        }

        /// <summary>
        /// Deletes a Review by its ID
        /// </summary>
        /// <param name="placeId">ID of the review's place</param>
        /// <param name="reservationId">ID of the review's reservation</param>
        /// <param name="reviewId">ID of the review</param>
        /// <returns>No content</returns>
        /// <response code="204">The Review was deleted.</response>
        /// <response code="400">The Review does not belong to the specified reservation.</response>
        /// <response code="404">The Review was not found.</response>
        [HttpDelete]
        [Route("Reservations/{reservationId}/[controller]/{reviewId}")]
        public async Task<ActionResult<IActionResult>> DeleteReview(int placeId, int reservationId, int reviewId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
                return NotFound("Place not found.");

            var reservation = await _context.Reservations
                                            .Include(r => r.Place)
                                            .FirstOrDefaultAsync(r => r.Id == reservationId);
            if (reservation == null)
                return NotFound("Reservation not found or does not belong to the specified place.");

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
                return NotFound("Review not found.");

            if (review.ReservationId != reservationId)
                return BadRequest("Review does not belong to the specified reservation.");

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
