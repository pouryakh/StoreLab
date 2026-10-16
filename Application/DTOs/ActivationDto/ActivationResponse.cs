using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ActivationDto
{
    public class ActivationResponse
    {
        public Guid ActivationRequestId { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }

        public List<ActivationProductResponse> Products { get; set; } = new();
    }
}
