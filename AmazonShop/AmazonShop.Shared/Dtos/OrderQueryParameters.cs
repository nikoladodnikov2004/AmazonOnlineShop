using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Shared.Dtos
{
    public class OrderQueryParameters
    {
        public string Status {  get; set; }=string.Empty;
        public int PageSize { get; set; } = 5;

        public int PageCount { get; set; } = 1;


    }
}
