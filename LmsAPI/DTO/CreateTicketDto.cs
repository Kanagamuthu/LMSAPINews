namespace LMSAPI.DTO
{
    public class CreateTicketDto
    {
        public string subject { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }

    public class TicketListDTO
    {
        public int? TicketId { get; set; }
        public string? Subject { get; set; }
        public string? Status { get; set; }
        // Support messages this student has not opened yet - the unread badge.
        public int UnreadCount { get; set; }
        public List<TicketMessageDTO>? Messages { get; set; }
    }

    public class TicketMessageDTO
    {
        public int? Id { get; set; }
        public string? Sender { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Read receipts. For a message the student sent, AdminRead is the tick the
        // student sees; for a support message, UserRead is the tick support sees.
        public bool AdminRead { get; set; }
        public bool UserRead { get; set; }

        /// <summary>
        /// Convenience for the chat bubble: "sent" (recipient has not opened it) or
        /// "read" (they have) - the single vs double-blue tick.
        /// </summary>
        public string? ReadStatus { get; set; }
    }

    public class TicketReplyDTO
    {
        public int ticketId { get; set; }
        public string message { get; set; }
    }

    public class TicketReadDTO
    {
        public int ticketId { get; set; }
    }

}
