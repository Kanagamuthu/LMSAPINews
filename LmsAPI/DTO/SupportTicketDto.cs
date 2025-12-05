namespace LMSAPI.DTO
{
    public class SupportTicketDto
    {
        public int ticketId { get; set; }
        public string subject { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string resolution { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public bool Status { get; set; }
    }
}
