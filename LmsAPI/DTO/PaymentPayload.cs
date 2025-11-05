using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PaymentPayload
    {
        public int? packageId { get; set; }
        public string? PaymentRefNo { get; set; }
        public string? PaymentStatus { get; set; }
        public string?  Type { get; set; }
    }
}
