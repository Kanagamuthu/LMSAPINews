namespace LMSAPI.DTO
{
    public class DepartmentSubjectDTO
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? SubjectId { get; set; } // nullable in case no subject exists
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectDescription { get; set; }
    }
}
