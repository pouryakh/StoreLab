using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem : BaseEntitiy
    {
        public Guid StoreOrderId { get; private set; }
        public Guid ProductId { get; private set; }

        public string ProductName { get; private set; } = null!;

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice => UnitPrice * Quantity;

        public StoreOrder StoreOrder { get; private set; } = null!;
        public Product Product { get; private set; } = null!;

        public OrderItem(Guid productId, Guid storeOrderId, string productName,int quantity,decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new DomainException("Price Should Be Grater Than 0");

            if(quantity <= 0)
                throw new DomainException("Quantity should be greater than 0");

            if(string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name cannot be empty.");

            if (productId == Guid.Empty)
                throw new DomainException("ProductId cannot be empty.");

            if (storeOrderId == Guid.Empty)
                throw new DomainException("StoreOrderId cannot be empty.");

            ProductId = productId;
            StoreOrderId = storeOrderId;
            UnitPrice = unitPrice;
            Quantity = quantity;
            ProductName = productName;
        }
    }
}
