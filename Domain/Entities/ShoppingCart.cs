using Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShoppingCart : BaseEntitiy
    {
        public Guid UserId { get; private set; }

        public User User { get; private set; } = null!;

        public ICollection<CartItem> Items { get; private set; }
            = new List<CartItem>();

        public ShoppingCart()
        {
        }

        public ShoppingCart(Guid userId)
        {
            UserId = userId;
        }

        public decimal CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach(CartItem item in Items)
            {
                totalAmount += item.TotalPrice;
            }
            return totalAmount;
        }

        public void AddItem(Product product,int quantity) 
        {
            if (quantity <= 0)
                throw new DomainException("Quantity Should be grater than zero");

            if (product.IsActive is false)
            {
                throw new DomainException("Product Is DeActived");
            }

            if (quantity > product.StockQuantity)
                throw new DomainException("Insufficient stock.");

            var item = Items.FirstOrDefault(x => x.ProductId == product.Id);

            if (item is not null)
            {
                item.IncreaseQuantity(quantity,product.StockQuantity);
                return;
            }

            Items.Add(new CartItem(product.Id,product.Name,product.Price,quantity,Id));
        }

        public void RemoveItem(Guid cartItemId)
        {
            var item = Items.FirstOrDefault(x => x.Id == cartItemId);

            if (item is null)
                throw new DomainException("Shopping cart dosent have this item");
            

            Items.Remove(item);
        }

        public void ChangeQuantity(int newQuantity,Product product)
        {
         
            var item = Items.FirstOrDefault(x => x.ProductId == product.Id);

            if (newQuantity == 0)
            {
                RemoveItem(product.Id);
                return;
            }

            if (product.IsActive == false)
            {
                throw new DomainException("Product Isnt Active");
            }

            if (item is null)
                throw new DomainException("Shopping cart dosent have this item");

            if (item.Quantity == newQuantity)
            {
                return;
            }

            if(item.Quantity <  newQuantity)
            {
                newQuantity -= item.Quantity;
                item.IncreaseQuantity(newQuantity, product.StockQuantity);
                return;
            }

            if (item.Quantity > newQuantity)
            {
                newQuantity -= item.Quantity;
                item.DecreaseQuantity(-newQuantity);
                return;
            }
        }

        public void ClearCart() 
        {
            Items.Clear();
        }
    }
}
