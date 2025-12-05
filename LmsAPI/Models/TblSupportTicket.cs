using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblSupportTicket
{
    public int StId { get; set; }

    public string? Name { get; set; }

    public string? Subject { get; set; }

    public string? EmailId { get; set; }

    public string? Message { get; set; }

    public DateTime? Createdon { get; set; }

    public int? Isread { get; set; }

    public DateTime? Readon { get; set; }

    public bool? ActiveStatus { get; set; }

    public int? ReadBy { get; set; }

    public string? Resolution { get; set; }
}
