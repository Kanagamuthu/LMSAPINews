using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class SupportTicket
{
    public int TicketId { get; set; }

    public string? Subject { get; set; }

    public string? EmailId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public bool? Status { get; set; }
}
