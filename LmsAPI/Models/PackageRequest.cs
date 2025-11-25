namespace LMSAPI.Models
{
    public class PackageRequest
    {
        public int EduType { get; set; }
        public int DepartmentId { get; set; }
        public string? PackageName { get; set; }
        public int PackageDurationDays { get; set; }
        public int Amount { get; set; }
        public string? CoverPath { get; set; }
        public List<SubjectDto>? Subjects { get; set; }
    }
    public class SubjectDto
    {
        public int SubjectId { get; set; }
    }
}
