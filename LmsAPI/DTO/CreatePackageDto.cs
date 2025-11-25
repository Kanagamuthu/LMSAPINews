namespace LMSAPI.DTO
{
    public class CreatePackageDto
    {
        
        //public int edu_type { get; set; }
        //public int DepartmentId { get; set; }
        public string? PackageCode { get; set; }
        public string? PackageName { get; set; }
        //public string? PackageDisplayName { get; set; }
        public int PackageDurationDays { get; set; }
        public int amount { get; set; }
        public string? CoverPath { get; set; }
        public List<PackageSubjectDto>? Subjects { get; set; }
    }
}
