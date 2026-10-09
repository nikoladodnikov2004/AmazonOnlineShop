using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Shared.Dtos
{
    public class WishListItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Brand { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string ImageUrl { get; set; }
        

    }
}
