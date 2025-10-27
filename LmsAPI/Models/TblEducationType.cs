using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblEducationType
{
    public int EduId { get; set; }

    public string? EduCode { get; set; }

    public string? EduDes { get; set; }

    public int? IsActive { get; set; }
}
