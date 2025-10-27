using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserSubjectActivationHistory
{
    public int UserSubjectActivationId { get; set; }

    public int? TusmId { get; set; }

    public int? SubjectId { get; set; }

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectVersion { get; set; }

    public int? UserId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? SubjectExpiryDate { get; set; }

    public DateTime? SubjectExpiryExtensionDate { get; set; }

    public int? SubjectExtensionDays { get; set; }

    public DateTime? ActivatedOn { get; set; }

    public int? ActivatedBy { get; set; }

    public int? ActivationType { get; set; }

    public int? ActivationProductType { get; set; }
}
