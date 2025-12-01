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

        public int Pagecontent { get; set; }

        public int? Solvedproblem { get; set; }

        public int? Multichoice { get; set; }

        public string? DeptVideo { get; set; }

        public int? IsInTrail { get; set; }

        public int? IsInDemo { get; set; }

        public int? TradeId { get; set; }

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
