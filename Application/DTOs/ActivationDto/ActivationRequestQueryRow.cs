using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivationDto
{
    public class ActivationRequestQueryRow
    {
        public Guid ActivationRequestId { get; set; }

        public Guid StoreId { get; set; }

        public string StoreName { get; set; } = null!;

        public string StoreDescription { get; set; } = null!;

        public int TotalCount { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }
    }
}
