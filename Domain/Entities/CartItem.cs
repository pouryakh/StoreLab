using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CartItem :BaseEntitiy
    {
        public Guid ShoppingCartId { get; private set; }
        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public ShoppingCart ShoppingCart { get; private set; } = null!;
        public Product Product { get; private set; } = null!;

        public decimal UnitPrice { get; private set; }
        public string ProductName { get; private set; }

        public decimal TotalPrice => UnitPrice * Quantity;

        public CartItem(Guid productId,string productName, decimal unitPrice, int quantity,Guid shoppingCartId)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity should be greater than 0");

            if (unitPrice <= 0)
                throw new DomainException("Price should be greater than 0");

            ShoppingCartId = shoppingCartId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            ProductId = productId;
        }

        public void IncreaseQuantity(int quantity,int stockQuantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity should be greater than 0");

            if (stockQuantity < Quantity + quantity)
                throw new DomainException("Insufficient stock.");

            Quantity += quantity;
        }

        public void DecreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity should be greater than 0");

            Quantity -= quantity;
        }
    }
}
