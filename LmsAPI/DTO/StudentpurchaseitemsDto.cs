namespace LMSAPI.DTO
{
    public class StudentpurchaseitemsDto
    {
        public int PackageId { get; set; }
        public string PackageCode { get; set; }
        public string packagedisplayname { get; set; }
        public int? SellingPrice { get; set; }
        public string? CoverPath { get; set; }
        public DateTime? SubjectExpiryDate { get; set; }
        public DateTime? PaymentOn { get; set; }
        
    }
}
