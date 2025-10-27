using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserRandomPass
{
    public long UserRandomId { get; set; }

    public int UserId { get; set; }

    public string VerificationCode { get; set; } = null!;

    public DateTime GeneratedTime { get; set; }

    public int ActionType { get; set; }

    public int UserType { get; set; }
}
