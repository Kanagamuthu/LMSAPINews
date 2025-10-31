using LMSAPI.Models;

namespace LMSAPI.Repository
{
    public interface IMeUserRepository
    {
        //add notification related methods here
        Task AddNotificationRecordAsync(TblUserNotificationDetail notification);  
    }
}
