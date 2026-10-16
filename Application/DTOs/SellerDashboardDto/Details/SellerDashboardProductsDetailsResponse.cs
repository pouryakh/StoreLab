using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SellerDashboardDto.Details
{
    public class SellerDashboardProductsDetailsResponse
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int LowStockProducts { get; set; }

        public decimal AverageProductPrice { get; set; }
        public decimal HighestProductPrice { get; set; }
        public decimal LowestProductPrice { get; set; }
    }
}
