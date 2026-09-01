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
    public class OrderController : ControllerBase
    {
        private readonly AmazonShopDbContext _context;
        public OrderController(AmazonShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders([FromQuery] OrderQueryParameters query)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var ordersQuery = _context.Orders
                .Where(o => o.UserId == userId)
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(query.Status) && query.Status != "All")
            {
                ordersQuery= ordersQuery.Where(o=>o.Status.ToLower() == query.Status.ToLower());
            }

            var ordersDto = await ordersQuery
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderReadDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status,
                    Items = o.OrderItems.Select(oi => new OrderItemReadDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        Name = oi.Product.Name,
                        Brand = oi.Product.Brand,
                        Quantity = oi.Quantity,
                        Price = oi.UnitPrice
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ordersDto);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderReadDto>> GetOrder(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderReadDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemReadDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Brand=oi.Product.Brand,
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            };

            return Ok(orderDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        
        public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto dto)
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

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Количката ви е празна.");
            }

            var order = new Order
            {
                UserId = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal totalPrice = 0;

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };
                order.OrderItems.Add(orderItem);
                totalPrice += cartItem.Product.Price*cartItem.Quantity;
            }

            order.TotalPrice = totalPrice;

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();


            var orderDto = new OrderReadDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemReadDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Name = oi.Product?.Name ?? "Unknown",
                    Brand = oi.Product?.Brand ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.UnitPrice
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
        }
        



        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = orderUpdateDto.Status;



            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
