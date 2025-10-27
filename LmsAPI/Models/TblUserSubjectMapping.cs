using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserSubjectMapping
{
    public long UserSubjectId { get; set; }

    public int SubjectId { get; set; }

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public string SubjectVersion { get; set; } = null!;

    public string? SubjectCoverPath { get; set; }

    public DateTime SubjectPurchasedon { get; set; }

    public int UserId { get; set; }

    public int DepartmentId { get; set; }

    public string? DepartmentCode { get; set; }

    public string? DepartmentName { get; set; }

    public int? DownloadStatus { get; set; }

    public DateTime DownloadedOn { get; set; }

    public int SubjectUnitId { get; set; }

    public string? SubjectUnitCode { get; set; }

    public string? SubjectUnitName { get; set; }

    public string SubjectUnitVersion { get; set; } = null!;

    public string SubjectUnitUserVersion { get; set; } = null!;

    public DateTime SubjectUnitExpiryon { get; set; }

    public DateTime? SubjectTrialExpiryon { get; set; }

    public int? IsTrial { get; set; }

    public int? IsDemo { get; set; }

    public int IsUnitOrIndex { get; set; }

    public int? IsActive { get; set; }

    public string? SubjectUnitPath { get; set; }

    public int IsUpdatedToClient { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public DateTime? LastClientUpdatedOn { get; set; }

    public int? IsUpdatedSubunitToClient { get; set; }

    public DateTime? LastSubunitModifiedOn { get; set; }

    public DateTime? LastSubunitClientUpdatedOn { get; set; }

    public DateTime PackageExpirydate { get; set; }

    public int? IsPurchased { get; set; }

    public int? Yearsem { get; set; }
}
