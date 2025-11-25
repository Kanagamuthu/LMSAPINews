using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblEngineering
{
    public byte SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string SubjectCode { get; set; } = null!;

    public string? SubjectDescription { get; set; }

    public string SubjectCoverPath { get; set; } = null!;

    public string SubjectSyllabusPath { get; set; } = null!;
}
