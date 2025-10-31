using LMSAPI.Models;

namespace LMSAPI.Repository
{
    public class MeUserRepository:IMeUserRepository
    {
        private readonly LmsdbNewContext _context;
        public MeUserRepository(LmsdbNewContext context)
        {
            _context = context;
        }
        public async Task AddNotificationRecordAsync(TblUserNotificationDetail notification)
        {
            _context.TblUserNotificationDetails.Add(notification);
            await _context.SaveChangesAsync();
        }

    }
}
