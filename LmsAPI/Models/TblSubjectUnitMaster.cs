using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblSubjectUnitMaster
{
    public int UnitId { get; set; }

    public string? UnitCode { get; set; }

    public string? UnitName { get; set; }

    public long? SubjectId { get; set; }

    public int? ActiveStatus { get; set; }

    public string? SubjectUnitPath { get; set; }

    public int? IsDemo { get; set; }

    public int? IsUnitOrIndex { get; set; }

    public int? SubjectUnitType { get; set; }

    public string? SubjectUnitVersion { get; set; }

    public DateTime? ReleasedOn { get; set; }

    public DateTime? CreatedOn { get; set; }

    public int? FilesizeInMb { get; set; }

    public int? CreatedBy { get; set; }

    public int? OsType { get; set; }
}
