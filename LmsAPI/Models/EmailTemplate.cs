using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class EmailTemplate
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Subject { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? Isdelete { get; set; }
}
