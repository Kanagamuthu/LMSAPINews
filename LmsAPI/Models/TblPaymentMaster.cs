using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblPaymentMaster
{
    public int PaymentMasterId { get; set; }

    public long? UserSubscribeMasterId { get; set; }

    public string? TxnMsg { get; set; }

    public string? TxnErrMsg { get; set; }

    public string? ClintTxnRefNo { get; set; }

    public string? TpsltxnBankCode { get; set; }

    public string? TpsltxnId { get; set; }

    public decimal? TxnAmount { get; set; }

    public DateTime? TxnDate { get; set; }

    public DateTime? TxnDateTime { get; set; }

    public string? TxnStatus { get; set; }

    public TimeOnly? TxnTime { get; set; }

    public string? TransactionType { get; set; }

    public decimal? TpslCharges { get; set; }

    public string? RpstToken { get; set; }

    public decimal? ServiceFee { get; set; }

    public decimal? BaseFee { get; set; }

    public string? TpslrefundId { get; set; }

    public decimal? BalAmt { get; set; }

    public string? RequestToken { get; set; }

    public int? SmsStatus { get; set; }

    public int? PaymentGatwayId { get; set; }

    public decimal? RefundAmt { get; set; }

    public DateTime? RefundDate { get; set; }
}
