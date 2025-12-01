using LMSAPI.Models;
using LMSAPI.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using static LMSAPI.DTO.LessonConverter;


namespace LMSAPI.DTO
{
    public partial class SubjectMasterDto
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDashboardRepository _dashboardRepository;
        private readonly LmsdbNewContext _context;

        public SubjectMasterDto(IHttpContextAccessor httpContextAccessor, IDashboardRepository dashboardRepository, LmsdbNewContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _dashboardRepository = dashboardRepository;
            _context = context;
        }
        public string SubjectId { get; set; }

        public string SubjectCode { get; set; } = null!;

        public string? UnivSubjectCode { get; set; }

        public string SubjectName { get; set; } = null!;

        public string? SubjectCoverPath { get; set; }

        public string? SubjectDescription { get; set; }

        public string ActiveStatus { get; set; }

        public string? RuleId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ReleasedOn { get; set; }

        public string UniversityId { get; set; }

        public string HavingQuestionpaper { get; set; }

        public string SubjectVersion { get; set; } = null!;

        public string ActiveDurationDays { get; set; }

        public DateTime ActiveDurationDate { get; set; }

        public string? Syllabus { get; set; }

        public string? DeptImgPath { get; set; }

        public string? Coursehours { get; set; }

        public string? Visuals { get; set; }

        public string Pagecontent { get; set; }

        public string? Solvedproblem { get; set; }

        public string? Multichoice { get; set; }

        public string? DeptVideo { get; set; }

        public string? Isstringrail { get; set; }

        public string? IsInDemo { get; set; }

        public string? TradeId { get; set; }

        public string? SubjectSyllabusPath { get; set; }

        
        public List<UnitDto>? Units { get; set; }

        //private Subject? _subjectSyllabus;
        //public Subject? SubjectSyllabus
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(SubjectSyllabusPath))
        //            return null;
        //        LessonConverter obj = new LessonConverter();
        //        var getdata = _dashboardRepository.GetAllReadHistory() ?? null;
        //        var userId = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        //        _subjectSyllabus = obj.GetLessonConverterAsync(SubjectSyllabusPath, userId, getdata).Result;
        //        //Pagecontent = _subjectSyllabus?.TotalChapters ?? 0;
        //        return _subjectSyllabus;
        //    }
        //}
    }
}
