using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductDto
{
    public class ChangeStockRequest
    {
        public Guid ProductId { get; set; }

        public int NewStock { get; set; }
    }
}
