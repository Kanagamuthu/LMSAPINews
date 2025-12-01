using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblSubjectChapterPath
{
    public int ScId { get; set; }

    public string Chaptrlink { get; set; } = null!;

    public string SubCode { get; set; } = null!;

    public int? IsActive { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }
}
