using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductDto
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public decimal? Price { get; set; } = null;
    }
}
