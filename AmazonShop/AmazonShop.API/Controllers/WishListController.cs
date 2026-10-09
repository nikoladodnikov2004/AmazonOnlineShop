using System.Security.Claims;
using AmazonShop.Domain.Entities;
using AmazonShop.Infrastructure.Data;
using AmazonShop.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmazonShop.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly AmazonShopDbContext _context;


        public WishListController(AmazonShopDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<WishListItemDto>>> GetWishlist()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var wishListItems = await _context.WishListItems
                .Where(c => c.UserId == userId)
                .Select(c => new WishListItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    Name = c.Product.Name,
                    Brand = c.Product.Brand,
                    Category = c.Product.Category.Name,
                    Price = c.Product.Price,
                    StockQuantity = c.Product.StockQuantity,
                    ImageUrl = c.Product.ImageUrl
                })
                .ToListAsync();

            return Ok(wishListItems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<WishListItemDto>> GetWishListItem(int id)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var wishListItem = await _context.WishListItems.Include(c => c.Product)
        .ThenInclude(c => c.Category).FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (wishListItem == null)
            {
                return NotFound();
            }


            return new WishListItemDto()
            {
                Id = wishListItem.Id,
                ProductId = wishListItem.ProductId,
                Name = wishListItem.Product.Name,
                Brand = wishListItem.Product.Brand,
                Category = wishListItem.Product.Category.Name,
                Price = wishListItem.Product.Price,
                StockQuantity = wishListItem.Product.StockQuantity,
                ImageUrl = wishListItem.Product.ImageUrl
            };
        }

       


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> DeleteWishListItem(int id)
        {


            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var wishListItem = await _context.WishListItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (wishListItem == null)
            {
                return NotFound();
            }
            _context.WishListItems.Remove(wishListItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> clearWishList()
        {


            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var wishListItems = await _context.WishListItems
                .Where(c => c.UserId == userId)
                .ToListAsync();



            _context.WishListItems.RemoveRange(wishListItems);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
