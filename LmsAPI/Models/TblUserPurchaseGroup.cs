using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserPurchaseGroup
{
    public int GroupId { get; set; }

    public string? SubjectCode { get; set; }

    public decimal? Price { get; set; }

    public int? ValidityDays { get; set; }

    public int? IsActive { get; set; }
}
