using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblDegreeMaster
{
    public int DId { get; set; }

    public string? DegreeType { get; set; }

    public DateTime? CrdOn { get; set; }

    public string? CrdBy { get; set; }

    public int? IsActive { get; set; }
}
