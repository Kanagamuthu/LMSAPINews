using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserNotificationMaster
{
    public int NotificationId { get; set; }

    public string? NotificationMessage { get; set; }

    public int? NotificationFor { get; set; }

    public int? Isread { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
}
