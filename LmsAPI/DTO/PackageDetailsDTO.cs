using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PackageDetailsDTO
    {
        public int? DepartmentSubjectMappingId { get; set; }
        public int? DepartmentId { get; set; }
        //public int? PackageDetailId { get; set; }
        public int? PackageId { get; set; }
        public int? Price { get; set; }
        //public string? DepartmentCode { get; set; }
        //public string? DepartmentName { get; set; }
        public List<SubjectMasterDto> SubjectMaster { get; set; }
    }
}
