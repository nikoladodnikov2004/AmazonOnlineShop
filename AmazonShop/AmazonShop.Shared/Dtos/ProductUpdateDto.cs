using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Shared.Dtos
{
    public class ProductUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        = string.Empty;

        [Required, MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Range(0.01, 100000.00)]    
        public decimal Price { get; set; }

        [Range(0, 100000)]
        public int StockQuantity { get; set; }
    }
}
