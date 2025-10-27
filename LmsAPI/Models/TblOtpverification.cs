using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblOtpverification
{
    public int OtpId { get; set; }

    public int? UId { get; set; }

    public string? Email { get; set; }

    public string? Otp { get; set; }

    public DateTime? ExpiryTime { get; set; }

    public bool? IsVerified { get; set; }
}
