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
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("place/{placeId}")]
        [SwaggerOperation(Summary = "Gets all Reviews for a specific Place", Description = "Gets all Reviews for a specific Place from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reviews were found.", typeof(IEnumerable<ReviewDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Place was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews(int placeId)
        {
            var place = await _context.Places.FindAsync(placeId);
            if (place == null)
            {
                return NotFound("Place not found.");
            }

            Console.WriteLine(placeId);

            var reviews = await _context.Reviews
                                        .Where(r => _context.Reservations
                                                            .Any(res => res.Place.Id == placeId))
                                        .ToListAsync();

            Console.WriteLine(reviews.Count);

            var reviewDtos = reviews.Select(r => new ReviewDto(
                r.Id,
                r.ReservationId,
                r.Rating,
                r.Comment
            )).ToList();

            return Ok(reviewDtos);
        }

        [HttpGet]
        [Route("reservation/{reservationId}")]
        [SwaggerOperation(Summary = "Gets a Review by Reservation ID", Description = "Gets a Review by Reservation ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was found.", typeof(ReviewDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReviewDto>> GetReviewByReservationId(int reservationId)
        {
            var review = await _context.Reviews
                                      .Where(r => r.ReservationId == reservationId)
                                      .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound();
            }

            var dto = new ReviewDto(
                review.Id,
                review.ReservationId,
                review.Rating,
                review.Comment
            );

            return dto;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a Review by ID", Description = "Gets a Review by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was found.", typeof(ReviewDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            var dto = new ReviewDto(
                review.Id,
                review.ReservationId,
                review.Rating,
                review.Comment
            );

            return dto;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new Review", Description = "Creates a new Review in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The Review was created.", typeof(ReviewDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A review for this reservation already exists.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReviewDto>> CreateReview([Validate] CreateReviewDto createReviewDto)
        {
            var review = new Review
            {
                ReservationId = createReviewDto.ReservationId,
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment
            };

            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            var existingReview = await _context.Reviews
                                        .AnyAsync(r => r.ReservationId == review.ReservationId);

            if (existingReview)
            {
                return BadRequest("A review for this reservation already exists.");
            }

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateReview), new { id = review.Id }, review);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a Review by ID", Description = "Updates a Review by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was updated.", typeof(ReviewDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Review is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<ReviewDto>> UpdateReview(int id, UpdateReviewDto updateReviewDto)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;

            await _context.SaveChangesAsync();

            return new ReviewDto(
                review.Id,
                review.ReservationId,
                review.Rating,
                review.Comment
            );

        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }

        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Deletes a Review by ID", Description = "Deletes a Review by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The Review was deleted.")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
