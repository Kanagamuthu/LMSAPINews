using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblDegreeDepartmentMapping
{
    public int Mapid { get; set; }

    public int? Degreeid { get; set; }

    public int? Departid { get; set; }

    public int? IsActive { get; set; }
}
