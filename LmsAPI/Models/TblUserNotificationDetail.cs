using System;
using System.Collections.Generic;

namespace LMSAPI.Models;

public partial class TblUserNotificationDetail
{
    public int UserNotifyId { get; set; }

    public int? UserId { get; set; }

    public int? Isread { get; set; }

    public int? NotificationId { get; set; }

    public DateTime? NotificationOn { get; set; }
}
