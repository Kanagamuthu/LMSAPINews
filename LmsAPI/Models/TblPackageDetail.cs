using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblPackageDetail
{
    public int PackageDetailId { get; set; }

    public int? PackageId { get; set; }

    public long? SubjectId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public int? ValidityDays { get; set; }

    public int? DepartmentId { get; set; }

    public int? Year { get; set; }

    public int? Semester { get; set; }

    public int? SubjectUnitType { get; set; }

    public int? DepartmentSubjectMappingId { get; set; }
}
