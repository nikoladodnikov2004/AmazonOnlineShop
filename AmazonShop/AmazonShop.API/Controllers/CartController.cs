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
    public class CartController : ControllerBase
    {
        private readonly AmazonShopDbContext _context;
        

        public CartController(AmazonShopDbContext context)
        {
            _context = context;
            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Select(c => new CartItemDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Name = c.Product.Name,
                    Brand = c.Product.Brand,
                    Category = c.Product.Category.Name,
                    Price = c.Product.Price,
                    StockQuantity = c.Product.StockQuantity,
                    ImageUrl = c.Product.ImageUrl
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CartItemDto>> GetCartItem(int id)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var cartItem = await _context.CartItems.Include(c => c.Product)
        .ThenInclude(c => c.Category).FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }


            return new CartItemDto()
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                Name = cartItem.Product.Name,
                Brand = cartItem.Product.Brand,
                Category = cartItem.Product.Category.Name,
                Price = cartItem.Product.Price,
                StockQuantity = cartItem.Product.StockQuantity,
                ImageUrl = cartItem.Product.ImageUrl
            };
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddToCart(AddToCartDto addToCart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var productExists = await _context.Products.AnyAsync(p => p.Id == addToCart.ProductId);
            if (!productExists)
            {
                return BadRequest("Продуктът не съществува.");
            }

            var existingItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == addToCart.ProductId);

            if(existingItem != null)
            {
                existingItem.Quantity += addToCart.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = addToCart.ProductId,
                    Quantity = addToCart.Quantity
                };
                _context.CartItems.Add(cartItem);
            }
                
            
            await _context.SaveChangesAsync();
            return Ok();
           
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCartItemQuantity(int id, CartItemQuantityUpdateDto cartItemUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c=> c.Id==id && c.UserId==userId);

            if (cartItem == null)
            {
                return NotFound();
            }
            

            if (cartItemUpdateDto.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = cartItemUpdateDto.Quantity;
            }

                await _context.SaveChangesAsync();

                return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> DeleteCartItem(int id)
        {
            

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (cartItem == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> clearCart()
        {


            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }


            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
           

            
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
