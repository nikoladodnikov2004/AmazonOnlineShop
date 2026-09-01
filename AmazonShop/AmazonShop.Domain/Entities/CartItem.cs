using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId {  get; set; }

        public int ProductId {  get; set; }

        [Range(1,100)]
        public int Quantity { get; set; } = 1;

        public User User { get; set; } = null;
        public Product Product { get; set; } = null;

    }
}
