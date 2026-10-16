using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Store : BaseEntitiy
    {
        public Guid SellerId { get; private set; }

        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;

        public bool IsActive { get; private set; }
        public StoreApprovalStatus Approval { get; private set; }

        public User Seller { get; private set; } = null!;

        public ICollection<Product> Products { get; private set; }
            = new List<Product>();

        public ICollection<StoreOrder> Orders { get; private set; }
            = new List<StoreOrder>();

        public Store()
        {
        }

        public Store(Guid sellerId,string name,string description) 
        {
            SellerId = sellerId;
            Name = name.ToLower();
            Description = description;
            IsActive = false;
            Approval = StoreApprovalStatus.Draft;
        }

        public void UpdateDetails(string? name,string? description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name.ToLower();
                IsActive = false;
                Approval = StoreApprovalStatus.Draft;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                Description = description;
            }
        }


        public void SubmitForApproval()
        {
            Approval = StoreApprovalStatus.PendingApproval;
        }

        public void Approve()
        {
            Approval = StoreApprovalStatus.Active;
            IsActive= true;
        }

        public void Reject()
        {
            Approval = StoreApprovalStatus.Rejected;
        }

        public int CountProducts()
        {
            return Products.Count;
        }

        public bool ActivationRequestCanBeSend()
        {
            if (CountProducts() >= 5)
            {
                return true;
            }

            return false;
        }

        public void MakeStoreDeactive()
        {
            IsActive = false;
            Approval = StoreApprovalStatus.Draft;
        }
    }
}
