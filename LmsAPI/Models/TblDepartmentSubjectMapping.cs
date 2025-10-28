using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblDepartmentSubjectMapping
{
    public int DepartmentSubjectMappingId { get; set; }

    public int? DepartmentId { get; set; }

    public long? SubjectId { get; set; }

    public int? RuleId { get; set; }

    public int? UniversityId { get; set; }

    public int? MapYear { get; set; }

    public int? Semester { get; set; }

    public int? ActiveStatus { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
}
