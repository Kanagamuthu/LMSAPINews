using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string? EmailId { get; set; }

    public string? CounteryCode { get; set; }

    public string? PhoneNumber { get; set; }

    public int? RoleId { get; set; }

    public bool? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDatetime { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDatetime { get; set; }
}
