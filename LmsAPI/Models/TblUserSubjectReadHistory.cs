using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserSubjectReadHistory
{
    public long UserReadHistoryId { get; set; }

    public int UserId { get; set; }

    public int? DepartmentId { get; set; }

    public int SubjectId { get; set; }

    public int TotalHours { get; set; }

    public DateTime LastReadOn { get; set; }

    public int? IsUpdatedToServer { get; set; }

    public string? HoursTopicVideo { get; set; }

    public DateTime? EnteredOn { get; set; }
}
