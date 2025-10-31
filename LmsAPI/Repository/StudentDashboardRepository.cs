using LMSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSAPI.Repository
{
    public class StudentDashboardRepository: IStudentDashboard
    {
        private readonly LmsdbNewContext _context;
        public StudentDashboardRepository(LmsdbNewContext context)
        {
            _context = context;
        }
        public async Task<object> GetStudentDashboardAsync(int userId)
        {
            // Implement the logic to retrieve student dashboard data from the database

            var result = await (from h in _context.TblUserSubjectActivationHistories
                                join s in _context.TblSubjectMasters
                                    on h.SubjectCode equals s.SubjectCode
                                where h.UserId == userId
                                select new
                                {
                                    h.UserId,
                                    h.SubjectCode,
                                    s.SubjectName,
                                    h.ActivatedOn,
                                    h.SubjectExpiryDate,
                                    s.ActiveDurationDays
                                }).ToListAsync();

            return result;

        }
    }
}
