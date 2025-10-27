using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblPersonalInformation
{
    public int UId { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? ProfileType { get; set; }

    public string Education { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? College { get; set; }

    public string? Department { get; set; }

    public int? BatchYear { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? IsActive { get; set; }

    public string? Trades { get; set; }

    public string? DeviceMacId { get; set; }

    public string? Platform { get; set; }

    public string? UserStatus { get; set; }

    public string? CountryCode { get; set; }
}
