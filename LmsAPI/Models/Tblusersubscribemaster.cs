using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserSubscribeMaster
{
    public int UserSubscribeMasterId { get; set; }

    public long? UserId { get; set; }

    public string? PaymentRefNo { get; set; }

    public decimal? Amount { get; set; }

    public int? Count { get; set; }

    public long? DiscountId { get; set; }

    public int? DiscountAmt { get; set; }

    public int? TransactionType { get; set; }

    public string? PaymentStatus { get; set; }

    public int? OrderStatus { get; set; }

    public int? UserSubscribeDeliveryModeId { get; set; }

    public DateTime? PaymentOn { get; set; }

    public DateTime? CreatedOn { get; set; }

    public long? BillingAddressId { get; set; }

    public long? DeliverAddressId { get; set; }

    public DateTime? RefundOn { get; set; }

    public int? RefundAmount { get; set; }

    public long? RefundBy { get; set; }
}
