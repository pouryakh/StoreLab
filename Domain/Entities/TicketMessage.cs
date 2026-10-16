using Domain.Common.Exceptions;

namespace Domain.Entities
{
    public class TicketMessage : BaseEntitiy
    {
        public Guid TicketId { get; private set; }

        public Guid SenderId { get; private set; }

        public string Message { get; private set; } = null!;

        public Ticket Ticket { get; private set; } = null!;
        public User Sender { get; private set; } = null!;

        public TicketMessage(
            Guid ticketId,
            Guid senderId,
            string message)
        {
            if (ticketId == Guid.Empty)
                throw new DomainException("Ticket is required.");

            if (senderId == Guid.Empty)
                throw new DomainException("Sender is required.");

            if (string.IsNullOrWhiteSpace(message))
                throw new DomainException("Message cannot be empty.");

            TicketId = ticketId;
            SenderId = senderId;
            Message = message.Trim();
        }

        public TicketMessage()
        {
        }
    }
}