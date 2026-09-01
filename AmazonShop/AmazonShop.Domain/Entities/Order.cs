using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId {  get; set; }

        public DateTime OrderDate { get; set; }=DateTime.UtcNow;

        [Column(TypeName="decimal(18,2)")]
        public decimal TotalSum { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
