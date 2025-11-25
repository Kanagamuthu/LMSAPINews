using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedDatetime { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDatetime { get; set; }
}
