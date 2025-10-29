using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static LMSAPI.DTO.LessonConverter;

namespace LMSAPI.Models;

public partial class TblSubjectMaster
{
    public long SubjectId { get; set; }

    public string SubjectCode { get; set; } = null!;

    public string? UnivSubjectCode { get; set; }

    public string SubjectName { get; set; } = null!;

    public string? SubjectCoverPath { get; set; }

    public string? SubjectDescription { get; set; }

    public int ActiveStatus { get; set; }

    public int? RuleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ReleasedOn { get; set; }

    public int UniversityId { get; set; }

    public int HavingQuestionpaper { get; set; }

    public string SubjectVersion { get; set; } = null!;

    public int ActiveDurationDays { get; set; }

    public DateTime ActiveDurationDate { get; set; }

    public string? Syllabus { get; set; }

    public string? DeptImgPath { get; set; }

    public int? Coursehours { get; set; }

    public int? Visuals { get; set; }

    public int? Pagecontent { get; set; }

    public int? Solvedproblem { get; set; }

    public int? Multichoice { get; set; }

    public string? DeptVideo { get; set; }

    public int? IsInTrail { get; set; }

    public int? IsInDemo { get; set; }

    public int? TradeId { get; set; }

    public string? SubjectSyllabusPath { get; set; }
}
