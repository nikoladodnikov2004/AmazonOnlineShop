using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId {  get; set; }

        public int ProductId {  get; set; }

        public int Quantity {  get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice {  get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
