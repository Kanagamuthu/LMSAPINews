using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblDiploma
{
    public byte SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string SubjectCode { get; set; } = null!;

    public string SubjectDescription { get; set; } = null!;

    public string SubjectCoverPath { get; set; } = null!;

    public string SubjectSyllabusPath { get; set; } = null!;
}
