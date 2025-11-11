using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblCountriesCode
{
    public int Id { get; set; }

    public string? CName { get; set; }

    public string? CCode { get; set; }

    public string? DialCode { get; set; }
}
