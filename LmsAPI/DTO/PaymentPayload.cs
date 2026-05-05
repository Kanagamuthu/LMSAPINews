using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PaymentPayload
    {
        public int? packageId { get; set; }
        public string? PaymentRefNo { get; set; }
        public string? PaymentStatus { get; set; }
        public string?  Type { get; set; }
        public string? price { get; set; }
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

    public class AppstorePayload
    {
        public int? packageId { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }



    //Apple Store
    public class IapVerifyRequest
    {
        public string productId { get; set; }
        public string transactionId { get; set; }
        public string receipt { get; set; }
        public string platform { get; set; }
        public string? price { get; set; }
        public string? currency { get; set; }
    }

    public class AppleVerifyResponse
    {
        public int status { get; set; }
        public AppleReceipt receipt { get; set; }
    }

    public class AppleReceipt
    {
        public List<AppleInApp> in_app { get; set; }
    }

    public class AppleInApp
    {
        public string product_id { get; set; }
        public string transaction_id { get; set; }
    }
    public class AppleVerifyResult
    {
        public bool IsValid { get; set; }
        public string ProductId { get; set; }
    }
}
