using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PackageDetailsDTO
    {
        public int? DepartmentSubjectMappingId { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? PackageId { get; set; }
        public string? PackageName { get; set; }
        public string? Coverpath { get; set; }
        public int? Price { get; set; }
        public string? Validity { get; set; }  
        public string? Validitydate { get; set; }  
        public List<SubjectMasterDto> SubjectMaster { get; set; }
    }
}
