using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblCountriesMaster
{
    public int Id { get; set; }

    public string? CommonName { get; set; }

    public string? OfficialName { get; set; }

    public string? FlagPng { get; set; }

    public string? FlagSvg { get; set; }

    public string? FlagAlt { get; set; }

    public string? IddRoot { get; set; }

    public string? IddSuffixes { get; set; }

    public int? IsActive { get; set; }
}
