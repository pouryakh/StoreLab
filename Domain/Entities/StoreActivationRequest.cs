using Domain.Common.Exceptions;
using Domain.Enums;

namespace Domain.Entities
{
    public class StoreActivationRequest : BaseEntitiy
    {
        public Guid StoreId { get; private set; }

        public Store Store { get; private set; } = null!;

        public StoreRequestStatus Status { get; private set; }

        public string? AdminNote { get; private set; }

        public DateTime? ReviewedAt { get; private set; }
        public StoreActivationRequest()
        {

        }

        public StoreActivationRequest(Guid storeId)
        {
            if (storeId == Guid.Empty)
                throw new DomainException("Store is required.");

            StoreId = storeId;
            Status = StoreRequestStatus.Pending;
        }

        public void Approve(string? adminNote = null)
        {
            if (Status != StoreRequestStatus.Pending)
                throw new DomainException("Only pending requests can be approved.");

            Status = StoreRequestStatus.Approved;
            AdminNote = adminNote?.Trim();
            ReviewedAt = DateTime.UtcNow;
        }

        public void Reject(string? adminNote = null)
        {
            if (Status != StoreRequestStatus.Pending)
                throw new DomainException("Only pending requests can be rejected.");

            Status = StoreRequestStatus.Rejected;
            AdminNote = adminNote?.Trim();
            ReviewedAt = DateTime.UtcNow;
        }

      
    }
}