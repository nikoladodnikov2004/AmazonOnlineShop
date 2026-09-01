using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Shared.Dtos
{
    public class OrderReadDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public DateTime OrderDate {  get; set; }
        public string Status { get; set; }=string.Empty;

        public decimal TotalPrice{ get; set; }

        public List<OrderItemReadDto> Items { get; set; }=new List<OrderItemReadDto>();
    }
}
