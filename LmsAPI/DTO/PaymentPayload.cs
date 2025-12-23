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
    public class PaymentRequest
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }

    public class VerifyRequest
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }

    }

    public class OrderStatus
    {
        public string OrderId { get; set; }
        public int userId { get; set; }
        public int PackageId { get; set; }

    }
}
