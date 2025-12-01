using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class SubjectChapter
{
    public int ChapterId { get; set; }

    public int SubjectId { get; set; }

    public int UnitId { get; set; }

    public string ChapterCode { get; set; } = null!;

    public string ChapterName { get; set; } = null!;

    public string? ChapterDescription { get; set; }

    public string ChapterPath { get; set; } = null!;

    public string? ChapterVersion { get; set; }

    public int? ChapterOrder { get; set; }

    public int ActiveStatus { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public int? UpdatedBy { get; set; }

    public int? FileSizeInMb { get; set; }

    public virtual TblSubjectMaster Subject { get; set; } = null!;

    public virtual SubjectUnit Unit { get; set; } = null!;
}
