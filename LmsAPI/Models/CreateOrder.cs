using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class CreateOrder
{
    public int Id { get; set; }

    public int? PackageId { get; set; }

    public string? OrderId { get; set; }

    public string? PaymentId { get; set; }

    public string? Signature { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Status { get; set; }

    public bool? IsDelete { get; set; }
}
