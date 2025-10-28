using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblSubjectMasterHistory
{
    public int SubjectHistoryid { get; set; }

    public long? SubjectId { get; set; }

    public string? SubjectCode { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectCoverPath { get; set; }

    public string? SubjectDescription { get; set; }

    public int? ActiveStatus { get; set; }

    public int? RuleId { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? ReleasedOn { get; set; }

    public int? UniversityId { get; set; }

    public int? HavingQuestionpaper { get; set; }

    public string? SubjectVersion { get; set; }

    public int? ActiveDurationDays { get; set; }

    public DateTime? ActiveDurationDate { get; set; }
}
