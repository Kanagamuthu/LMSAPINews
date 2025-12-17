using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class ChatKnowledgeBase
{
    public int Id { get; set; }

    public string? Question { get; set; }

    public string? Answer { get; set; }

    public string? Keywords { get; set; }

    public bool? IsActive { get; set; }
}
