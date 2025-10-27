using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblStudentTrialSubject
{
    public long UserTrialSubjectId { get; set; }

    public long UserId { get; set; }

    public long SubjectId { get; set; }

    public DateTime TrailExpiryOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? TradeActiveStatus { get; set; }
}
