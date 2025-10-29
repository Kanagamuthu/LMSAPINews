using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class DepartmentSubjectDTO
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public SubjectMasterDto? subjectMaster { get; set; }
    }
}
