using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblPackageMaster
{
    public int PackageId { get; set; }

    public string? PackageCode { get; set; }

    public string? PackageDisplayName { get; set; }

    public string? PackageName { get; set; }

    public int? PackageDurationDays { get; set; }

    public string? LongDesc { get; set; }

    public string? ShortDesc { get; set; }

    public string? SellingPrice { get; set; }

    public string? ActualPrice { get; set; }

    public int? DepartmentId { get; set; }

    public long? SubjectId { get; set; }

    public long? RuleId { get; set; }

    public int? SubjectUnitType { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public int? CurrentStatus { get; set; }

    public bool? Activestatus { get; set; }

    public int? IsOfferPackage { get; set; }

    public string? CoverPath { get; set; }

    public int? UnivId { get; set; }

    public int? Year { get; set; }

    public int? Semester { get; set; }

    public int? IsBundle { get; set; }

    public string? Keywords { get; set; }

    public string? PackageVideoUrl { get; set; }

    public int? OsType { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? Updateddate { get; set; }

    public int? Discount { get; set; }

    public string? Dealname { get; set; }

    public virtual ICollection<TblPackageDetail> TblPackageDetails { get; set; } = new List<TblPackageDetail>();
}
