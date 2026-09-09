using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Brand { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string ASIN { get; set; }
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }= string.Empty;

        public string Condition { get; set; }= string.Empty;

        public int StockQuantity { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CategoryId {  get; set; }
        public Category Category { get; set; } = null!;
        
        public double Rating { get; set; } = 0.0;
        public int ReviewCount { get; set; } = 0;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
