using Domain.Common.Exceptions;
using Domain.Enums;

namespace Domain.Entities
{
    public class Ticket : BaseEntitiy
    {
        public Guid UserId { get; private set; }

        public Guid? OrderId { get; private set; }
        public Guid? StoreOrderId { get; private set; }

        public string Title { get; private set; } = null!;

        public TicketStatus Status { get; private set; }
        public TicketPriority Priority { get; private set; }

        public DateTime? ClosedAt { get; private set; }

        public User User { get; private set; } = null!;
        public Order? Order { get; private set; }
        public StoreOrder? StoreOrder { get; private set; }

        public ICollection<TicketMessage> Messages { get; private set; }
            = new List<TicketMessage>();

        public Ticket(
            Guid userId,
            string title,
            Guid? orderId = null,
            Guid? storeOrderId = null)
        {
            if (userId == Guid.Empty)
                throw new DomainException("User is required.");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Ticket title cannot be empty.");

            UserId = userId;
            Title = title.Trim();

            OrderId = orderId;
            StoreOrderId = storeOrderId;

            Status = TicketStatus.Open;
            Priority = TicketPriority.Medium;
        }

        public void StartProgress()
        {
            if (Status != TicketStatus.Open)
                throw new DomainException("Invalid ticket transition.");

            Status = TicketStatus.InProgress;
        }

        public void WaitForUser()
        {
            if (Status != TicketStatus.InProgress)
                throw new DomainException("Invalid ticket transition.");

            Status = TicketStatus.WaitingForUser;
        }

        public void Resume()
        {
            if (Status != TicketStatus.WaitingForUser)
                throw new DomainException("Invalid ticket transition.");

            Status = TicketStatus.InProgress;
        }

        public void Resolve()
        {
            if (Status != TicketStatus.InProgress)
                throw new DomainException("Invalid ticket transition.");

            Status = TicketStatus.Resolved;
        }

        public void Close()
        {
            if (Status != TicketStatus.Resolved)
                throw new DomainException("Invalid ticket transition.");

            Status = TicketStatus.Closed;
            ClosedAt = DateTime.UtcNow;
        }

        public void ChangePriority(TicketPriority priority)
        {
            Priority = priority;
        }
    }
}