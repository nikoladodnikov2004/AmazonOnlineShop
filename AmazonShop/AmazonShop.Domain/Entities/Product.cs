using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AmazonShop.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string ASIN { get; set; }
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }= string.Empty;

        public string Condition { get; set; }= string.Empty;

        public int StockQuantity { get; set; } = 0;


        public int CategoryId {  get; set; }
        public Category Category { get; set; } = null!;


    }
}
