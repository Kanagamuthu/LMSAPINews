using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PackageDetailsDTO
    {
        public string? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? PackageId { get; set; }
        public string? PackageName { get; set; }
        public string? Coverpath { get; set; }
        public string? Price { get; set; }
        public string Validity { get; set; }  
        public DateTime? Validitydate { get; set; }  
        public bool IsPurchased { get; set; }
        public DateTime? SubjectExpiryDate { get; set; }
        public DateTime? PaymentOn { get; set; }
        public DateTime? serverdate { get; set; } = DateTime.Now;
        public List<SubjectMasterDto> SubjectMaster { get; set; }
    }
}
