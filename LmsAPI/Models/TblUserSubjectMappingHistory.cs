using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserSubjectMappingHistory
{
    public long UserSubjectHistoryId { get; set; }

    public long? UserSubjectId { get; set; }

    public int? SubjectId { get; set; }

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectVersion { get; set; }

    public string? SubjectCoverPath { get; set; }

    public int? UserId { get; set; }

    public int? DepartmentId { get; set; }

    public string? DepartmentCode { get; set; }

    public string? DepartmentName { get; set; }

    public int? DownloadStatus { get; set; }

    public DateTime? DownloadedOn { get; set; }

    public int? SubjectUnitId { get; set; }

    public string? SubjectUnitCode { get; set; }

    public string? SubjectUnitName { get; set; }

    public string? SubjectUnitVersion { get; set; }

    public DateTime? SubjectUnitExpiryon { get; set; }

    public int? IsDemo { get; set; }

    public int? IsUnitOrIndex { get; set; }

    public string? SubjectUnitPath { get; set; }
}
