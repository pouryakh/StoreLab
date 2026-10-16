using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ShoppingCartDto
{
    public class ShoppingCartResponse
    {
        public decimal totalAmount { get; set; }

        public ICollection<CartItemResponse> Items { get; set; }
            = new List<CartItemResponse>();
    }
}
