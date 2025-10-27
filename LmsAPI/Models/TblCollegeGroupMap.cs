using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblCollegeGroupMap
{
    public long CollegeGroupMapId { get; set; }

    public int CollegeId { get; set; }

    public int CollegeGroupId { get; set; }

    public DateTime EnterOn { get; set; }

    public int EnterBy { get; set; }
}
