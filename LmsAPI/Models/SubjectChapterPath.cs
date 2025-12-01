using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class SubjectChapterPath
{
    public int SubjectChapterPathId { get; set; }

    public int ChapterId { get; set; }

    public string ChapterLink { get; set; } = null!;

    public string SubjectCode { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }
}
