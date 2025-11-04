using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblDepartmentMaster
{
    public int DepartmentId { get; set; }

    public string DepartmentCode { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public int NoOfYear { get; set; }

    public int MaxSemesterPerYear { get; set; }

    public int ActiveStatus { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int UniversityId { get; set; }

    public string? DeptImgPath { get; set; }

    public int? Coursehours { get; set; }

    public int? Visuals { get; set; }

    public int? Pagecontent { get; set; }

    public int? Solvedproblem { get; set; }

    public int? Multichoice { get; set; }

    public string? DeptVideo { get; set; }

    public int? IsActive { get; set; }

    public int? DegreeId { get; set; }
}
