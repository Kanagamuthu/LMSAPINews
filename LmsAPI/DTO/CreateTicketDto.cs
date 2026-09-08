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
        public List<TicketMessageDTO>? Messages { get; set; }
    }

    public class TicketMessageDTO
    {
        public int? Id { get; set; }
        public string? Sender { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class TicketReplyDTO
    {
        public int ticketId { get; set; }
        public string message { get; set; }
    }

}
