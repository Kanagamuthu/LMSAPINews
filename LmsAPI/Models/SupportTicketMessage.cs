using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class SupportTicketMessage
{
    public int Id { get; set; }

    public int? TicketId { get; set; }

    public int? SenderId { get; set; }

    public string? Sender { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? AdminRead { get; set; }

    public bool? UserRead { get; set; }
}
