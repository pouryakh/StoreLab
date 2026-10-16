using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ShoppingCartDto
{
    public class UpdateItemQuantityRequest
    {
        public Guid ProductId { get; set; }

        public int NewQuantity { get; set; }
    }
}
