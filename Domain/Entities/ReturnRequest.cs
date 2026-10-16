using Domain.Common.Exceptions;
using Domain.Enums;

namespace Domain.Entities
{
    public class ReturnRequest : BaseEntitiy
    {

        public Guid StoreOrderId { get; private set; }
        public Guid UserId { get; private set; }

        public string Reason { get; private set; } = null!;

        public ReturnStatus Status { get; private set; }

        public DateTime RequestedAt { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public DateTime? ReceivedAt { get; private set; }

        public StoreOrder StoreOrder { get; private set; } = null!;

        public WalletTransaction? RefundTransaction { get; private set; }
        public User User { get; private set; } = null!;


        public ReturnRequest(
            Guid storeOrderId,
            Guid userId,
            string reason)
        {
            if (storeOrderId == Guid.Empty)
                throw new DomainException("Store order is required.");

            if (userId == Guid.Empty)
                throw new DomainException("User is required.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException(
                    "Return reason cannot be empty.");

            StoreOrderId = storeOrderId;
            UserId = userId;
            Reason = reason.Trim();

            Status = ReturnStatus.Requested;
            RequestedAt = DateTime.UtcNow;
        }

       

        public void Approve()
        {
            if (Status != ReturnStatus.Requested)
                throw new DomainException(
                    "Invalid return transition.");

            Status = ReturnStatus.Approved;
        }

        public void Reject()
        {
            if (Status != ReturnStatus.Requested)
                throw new DomainException(
                    "Invalid return transition.");

            Status = ReturnStatus.Rejected;
            ResolvedAt = DateTime.UtcNow;
        }

        public void MarkAsReceived()
        {
            //if (Status != ReturnStatus.Approved)
            //    throw new DomainException(
            //        "Return request must be approved first.");

            if (ReceivedAt.HasValue)
                throw new DomainException(
                    "Return has already been received.");

            ReceivedAt = DateTime.UtcNow;
            Status = ReturnStatus.PendingRefund;
        }

        public void MarkAsRefunded()
        {
            //if (Status != ReturnStatus.Approved)
            //    throw new DomainException(
            //        "Return request must be approved first.");

            if (!ReceivedAt.HasValue)
                throw new DomainException(
                    "Returned product has not been received.");

            Status = ReturnStatus.Refunded;
            ResolvedAt = DateTime.UtcNow;
        }
    }
}
