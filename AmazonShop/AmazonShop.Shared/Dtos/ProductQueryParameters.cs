using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonShop.Shared.Dtos
{
    public class ProductQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get=> _pageSize; set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
         }

    }
}
