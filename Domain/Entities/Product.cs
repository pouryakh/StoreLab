using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product : BaseEntitiy
    {
        public Guid StoreId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        public decimal Price { get; private set; }

        public int StockQuantity { get; private set; }

        public bool IsActive { get; private set; }

        public byte[] RowVersion { get; private set; } = null!;

        public Store Store { get; private set; } = null!;

        public Product()
        {
        }

        public Product(Guid storeId,string name,string discription,decimal price,int stockQuantity)
        {
            StoreId = storeId;
            Name = name; 
            Description = discription;
            Price = price;
            StockQuantity = stockQuantity;
        }

        public void SetPrice(decimal price)
        {
            if (price <= 0)
                throw new DomainException("Price should be greater than 0");

            Price = price;
        }

        public void Activate()
        {
            if (StockQuantity <= 0)
                throw new DomainException("Product cannot be activated without stock.");

            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("quantity should be greater than zero");
            
            StockQuantity += quantity;
        }

        public void DecreaseStock(int stock)
        {
            if (stock <= 0)
                throw new DomainException("StockAddNumber should be greater than zero");

            if (stock > StockQuantity)
                throw new DomainException("Insufficient stock.");

            StockQuantity -= stock;
        }

        public void UpdateDetails(
            string? name,
            string? description,
            decimal? price)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(description))
            {

                Description = description.Trim();
            }

            if (price.HasValue && price != 0)
            {
                if (price < 0)
                    throw new DomainException("Price must be greater than zero.");

                Price = price.Value;
            }

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
