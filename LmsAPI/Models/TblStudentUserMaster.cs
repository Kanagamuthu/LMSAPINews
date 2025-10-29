using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblStudentUserMaster
{
    public long StudentUserId { get; set; }

    public string? UserFirstName { get; set; }

    public string Username { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public int ActiveStatus { get; set; }

    public DateTime? AccActiveOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? Collegename { get; set; }

    public int? DepartmentId { get; set; }

    public string? DepartmentName { get; set; }

    public int? Batchyear { get; set; }

    public string? Country { get; set; }

    public string? PrimaryImei { get; set; }

    public string? PrimaryMac { get; set; }

    public string? CountryCode { get; set; }

    public int? EduType { get; set; }

    public int? TradeId { get; set; }

    public string? Token { get; set; }
}
