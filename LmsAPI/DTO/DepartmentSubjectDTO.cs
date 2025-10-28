using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class DepartmentSubjectDTO
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        //public int? SubjectId { get; set; } 
        //public string? SubjectName { get; set; }
        //public string? SubjectCode { get; set; }
        //public string? SubjectDescription { get; set; }
        public TblSubjectMaster? subjectMaster { get; set; }
    }
}
