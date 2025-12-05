using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblReadHistory
{
    public int Id { get; set; }

    public int? SubjectId { get; set; }

    public int? UnitId { get; set; }

    public int? ChapterId { get; set; }

    public string? Type { get; set; }

    public int? Readby { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Status { get; set; }
}
