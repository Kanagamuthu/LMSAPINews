using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblExceptionLog
{
    public int ExceptionId { get; set; }

    public string ExceptionType { get; set; } = null!;

    public string ExceptionMessage { get; set; } = null!;

    public string ExceptionSource { get; set; } = null!;

    public string? ExceptionUrl { get; set; }

    public string TargetSiteModule { get; set; } = null!;

    public string TargetSiteName { get; set; } = null!;

    public string StackTrace { get; set; } = null!;

    public string? HelpLink { get; set; }

    public bool? IsFromController { get; set; }

    public DateTime ExceptionDate { get; set; }

    public int? ErrorColumnNumber { get; set; }

    public int? ErrorLine { get; set; }
}
