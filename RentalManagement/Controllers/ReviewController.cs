using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RentalManagement.Contexts;
using RentalManagement.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace RentalManagement.Controllers
{
    // TODO: You should be able to get a list of all reviews for a specific place
    // TODO: You should be able to get a specific review by Reservation ID
    // TODO: You should be able to get a review by it's ID

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
        [SwaggerOperation(Summary = "Gets all Reviews for a specific Place", Description = "Gets all Reviews for a specific Place from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Reviews were found.", typeof(IEnumerable<Review>))]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews([FromQuery] int placeId)
        {
            return await _context.Reviews
                                    .Where(r => _context.Reservations
                                                        .Any(res => res.Id == r.ReservationId && res.Place.Id == placeId))
                                    .ToListAsync();
        }

        [HttpGet]
        [Route("reservation/{id}")]
        [SwaggerOperation(Summary = "Gets a Review by Reservation ID", Description = "Gets a Review by Reservation ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was found.", typeof(Review))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Review>> GetReviewByReservationId(int id)
        {
            var review = await _context.Reviews
                                      .Where(r => r.ReservationId == id)
                                      .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Gets a Review by ID", Description = "Gets a Review by ID from the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was found.", typeof(Review))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new Review", Description = "Creates a new Review in the database.")]
        [SwaggerResponse(StatusCodes.Status201Created, "The Review was created.", typeof(Review))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Review is invalid.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Review>> CreateReview(Review review)
        {
            // FIXME: You should be able to create only one review per reservation

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // return CreatedAtAction(nameof(CreateReview), new { id = review.Id }, review);
            return CreatedAtAction(nameof(CreateReview), review);
        }

        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(Summary = "Updates a Review by ID", Description = "Updates a Review by ID in the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The Review was updated.", typeof(Review))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "The Review is invalid.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The Review was not found.", typeof(ValidationProblemDetails))]
        public async Task<ActionResult<Review>> UpdateReview(int id, Review review)
        {
            if (id != review.Id)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return review;
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
