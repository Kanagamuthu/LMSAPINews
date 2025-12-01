using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class SubjectUnit
{
    public int UnitId { get; set; }

    public string UnitCode { get; set; } = null!;

    public string UnitName { get; set; } = null!;

    public long SubjectId { get; set; }

    public int ActiveStatus { get; set; }

    public string SubjectUnitPath { get; set; } = null!;

    public int? IsDemo { get; set; }

    public int? IsUnitOrIndex { get; set; }

    public int? SubjectUnitType { get; set; }

    public string? SubjectUnitVersion { get; set; }

    public DateTime? ReleasedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? FileSizeInMb { get; set; }

    public virtual ICollection<SubjectChapter> SubjectChapters { get; set; } = new List<SubjectChapter>();
}
