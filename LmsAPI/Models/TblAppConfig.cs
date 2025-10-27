using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblAppConfig
{
    public int CId { get; set; }

    public string? ConfigKey { get; set; }

    public string ConfigValue { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }
}
