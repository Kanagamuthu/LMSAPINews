using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class ReadTimeHistory
{
    public int Id { get; set; }

    public int? SubjectId { get; set; }

    public int? UnitId { get; set; }

    public int? ChapterId { get; set; }

    public int? Readby { get; set; }

    public TimeOnly? ReadTime { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? Status { get; set; }
}
