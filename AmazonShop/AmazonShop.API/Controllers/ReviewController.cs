using System.Security.Claims;
using AmazonShop.Domain.Entities;
using AmazonShop.Infrastructure.Data;
using AmazonShop.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmazonShop.API.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AmazonShopDbContext _context;
        public ReviewController(AmazonShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ReviewReadDto>>> GetReviewsByProduct(int productId)
        {


            var reviews = await _context.Reviews
                .Where(r => r.ProductId ==productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewReadDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    UserName = r.UserName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt

                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewReadDto>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return new ReviewReadDto()
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = review.UserName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt

            };
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewReadDto>> CreateReview(int productId, ReviewCreateDto reviewCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var userName=User.Identity?.Name ?? "Анонимен";

            var review = new Review
            {

                ProductId = productId,
                UserId = userId,
                UserName =userName,
                Rating = reviewCreateDto.Rating,
                Comment = reviewCreateDto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var product = await _context.Products
        .Include(p => p.Reviews)
        .FirstOrDefaultAsync(p => p.Id == productId);

            if (product != null)
            {
                product.ReviewCount = product.Reviews.Count;
                product.Rating = Math.Round(product.Reviews.Average(r => r.Rating), 1);
                await _context.SaveChangesAsync();
            }

            var reviewReadDto = new ReviewReadDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                UserName = review.UserName,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
               
                

    };
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewReadDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateReview(int id, ReviewUpdateDto reviewUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            review.Rating = reviewUpdateDto.Rating;
            review.Comment = reviewUpdateDto.Comment;

            var product = await _context.Products
                .Include(p =>p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == review.ProductId);

            if (product != null && product.Reviews.Any())
            {
                product.Rating = Math.Round(product.Reviews.Average(r => r.Rating), 1);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

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
