using AmazonShop.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmazonShop.Shared.Dtos;
using AmazonShop.Domain.Entities;

namespace AmazonShop.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AmazonShopDbContext _context;
        public ProductController(AmazonShopDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<ProductReadDto>>> GetProducts([FromQuery] ProductQueryParameters query)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(query.Name));
            }
            if (!string.IsNullOrWhiteSpace(query.Brand))
            {
                productsQuery = productsQuery.Where(p => p.Brand.Contains(query.Brand));
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                productsQuery = productsQuery.Where(p => p.Category != null && p.Category.Name.Contains(query.Category));
            }

            productsQuery = query.SortBy?.ToLower() switch
            {

                "brand" => query.SortDescending ? productsQuery.OrderByDescending(p => p.Brand) : productsQuery.OrderBy(p => p.Brand),
                "category" => query.SortDescending ? productsQuery.OrderByDescending(p => p.Category.Name) : productsQuery.OrderBy(p => p.Category.Name),
                "price" => query.SortDescending ? productsQuery.OrderByDescending(p => p.Price) : productsQuery.OrderBy(p => p.Price),
                _ => query.SortDescending ? productsQuery.OrderByDescending(p => p.Name) : productsQuery.OrderBy(p => p.Name)
            };

            var totalCount = await productsQuery.CountAsync();

            var skip = (query.Page - 1) * query.PageSize;
            var products = await productsQuery
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync();

            var result = products.Select(p => new ProductReadDto
            {

                Id = p.Id,
                Name = p.Name,
                Brand = p.Brand,
                Description = p.Description,
                Category = p.Category?.Name ?? string.Empty,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = p.ImageUrl

            });

            return Ok(new PagedResult<ProductReadDto>
            {
                Items = result,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            });
        
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProductReadDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return new ProductReadDto()
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Description = product.Description,
                Category = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = product.ImageUrl,
            };
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProductReadDto>> CreateProduct(ProductCreateDto productCreateDto)
        {
            string? imageURL = null;

            if (productCreateDto.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(productCreateDto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productCreateDto.ImageFile.CopyToAsync(fileStream);
                }

                imageURL = uniqueFileName;
            }

            var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Name == productCreateDto.Category);

            if (categoryEntity != null)
            {
                categoryEntity=new Category
                {
                    Name = productCreateDto.Category
                };
                _context.Categories.Add(categoryEntity);
                await _context.SaveChangesAsync();
            }

            var product = new Product
            {
                Name = productCreateDto.Name,
                Brand = productCreateDto.Brand,
                Description = productCreateDto.Description,
                ASIN = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(), // Автоматично генериране на ASIN
                CategoryId = categoryEntity.Id,
                Category = categoryEntity,
                Price = productCreateDto.Price,
                StockQuantity = productCreateDto.StockQuantity,
                ImageUrl = imageURL ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var productReadDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                Description = product.Description,
                Category = categoryEntity.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ImageUrl = imageURL,
            };
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, productReadDto);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto productUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Name == productUpdateDto.Category);

            if (categoryEntity != null)
            {
                categoryEntity = new Category
                {
                    Name = productUpdateDto.Category
                };
                _context.Categories.Add(categoryEntity);
                await _context.SaveChangesAsync();
            }

            product.Name = productUpdateDto.Name;
            product.Brand = productUpdateDto.Brand;
            product.Description = productUpdateDto.Description;
            product.CategoryId = categoryEntity.Id;
            product.Price = productUpdateDto.Price;
            product.StockQuantity = productUpdateDto.StockQuantity;
            
               
            await _context.SaveChangesAsync();
           
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
