
using LmsAPI.Models;
using LMSAPI.DTO;
using LMSAPI.Helpers;
using LMSAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace LMSAPI.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly LmsdbContext _context;
        private readonly ILoggerManager _logger;
        private readonly IStudentsRepository _studentsRepository;
        public DashboardRepository(LmsdbContext context, ILoggerManager logger, IStudentsRepository studentsRepository)
        {
            _context = context;
            _logger = logger;
            _studentsRepository = studentsRepository;
        }

        public bool IsValidStudent(string userEmail)
        {
            try
            {
                _logger.LogInfo("Checking if student is valid for email: " + userEmail);

                var student = _context.TblStudentUserMasters.Where(s => s.EmailId == userEmail && s.ActiveStatus == 1);
                if (student.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        Task<List<TblSubjectMaster>> IDashboardRepository.GetAllSubjects()
        {
            try
            {
                return _context.TblSubjectMasters.Where(s => s.ActiveStatus == 1).ToListAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;

            }
        }
        public async Task<bool> AddBooksToStudent(string userEmail, List<int> bookIds)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == userEmail && s.ActiveStatus == 1);
                if (student == null)
                {
                    _logger.LogWarn("Student not found for email: " + userEmail);
                    return false;
                }
                foreach (var bookId in bookIds)
                {
                    //trail period calculated from table TblStudentUserMaster - acc acvivet on
                    var trialDays = await _studentsRepository.GetTrialPeriodDaysAsync();
                    //get activate date from table
                    var stdmaster = await _studentsRepository.GetStudentByEmailAsync(userEmail);
                    var activationDate = stdmaster.AccActiveOn.Value;
                    var currentDate = DateTime.UtcNow;
                    var difference = currentDate - activationDate;
                    int daysSinceActivation = (int)difference.TotalDays;
                    int daysLeft = trialDays - daysSinceActivation; //days left date calculation 
                    DateTime trail_expairy_on = activationDate.AddDays(trialDays);

                    var existingEntry = await _context.TblStudentTrialSubjects.FirstOrDefaultAsync(m => m.SubjectId == student.StudentUserId && m.SubjectId == bookId);
                    if (existingEntry == null)
                    {
                        var mapping = new TblStudentTrialSubject
                        {
                            UserId = student.StudentUserId,
                            SubjectId = bookId,
                            TrailExpiryOn = trail_expairy_on,
                            CreatedOn = DateTime.Now
                        };
                        await _context.TblStudentTrialSubjects.AddAsync(mapping);
                    }
                }
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                _logger.LogError(ex, "Error occurred while adding books to student: " + userEmail);
                return false;
            }
        }
        public async Task<int> GetBookLimitPerStudentAsync()
        {
            try
            {
                var booklimitSetting = await _context.TblAppConfigs.ToListAsync();
                if (booklimitSetting != null && booklimitSetting.Count > 0)
                {
                    var bookLimitConfig = booklimitSetting.FirstOrDefault(c => c.ConfigKey == "max_books_per_trial");
                    if (bookLimitConfig != null && int.TryParse(bookLimitConfig.ConfigValue, out int bookLimit))
                    {
                        return bookLimit;
                    }
                    else
                    {
                        _logger.LogWarn("Book limit setting not found or invalid in application configuration.");
                        return 5;
                    }

                }
                else
                {
                    _logger.LogWarn("Book limit setting not found in application configuration.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving book limit per student.");
                throw;
            }
        }
        public async Task<int> GetCurrentBookCountForStudentAsync(int studentUserId)
        {
            try
            {
                var currentBookCount = await _context.TblStudentTrialSubjects.CountAsync(s => s.UserId == studentUserId);
                return currentBookCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current book count for student ID: " + studentUserId);
                throw;
            }
        }

        public async Task SetInactive(long sud)
        {
            try
            {
                await _context.TblStudentTrialSubjects.Where(s => s.UserId == sud).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.TradeActiveStatus, 0));
            }
            catch (Exception er)
            {
                _logger.LogError(er.InnerException);
                throw;
            }
        }

        public async Task<List<TblStudentTrialSubject>> GetActiveTradesByUserIDAsync(long userId)
        {
            return await _context.TblStudentTrialSubjects
                .Where(t => t.UserId == userId && t.TradeActiveStatus == 1) // active trades only
                .ToListAsync();
        }
        public async Task<bool> PostRegisterStudentTradeDepartment(string userEmail, StudentTradeDepartmentDTO studentTradeDepartmentDTO)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == userEmail);
                if (student == null)
                {
                    return false;
                }
                // update the department
                student.DepartmentId = studentTradeDepartmentDTO.DepartmentId;
                student.TradeId = studentTradeDepartmentDTO.TradeId;

                // mark entity as modified and save

                _context.TblStudentUserMasters.Update(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering student trade and department");
                return false;
            }
        }
        public async Task<List<StudentTradeDepartmentDTO>> GetSubjectsByStudentTrade(string userEmail, int tradeID)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == userEmail);
                if (student == null)
                {
                    return new List<StudentTradeDepartmentDTO>();
                }
                var subjects = await (from stu in _context.TblStudentUserMasters
                                      join sub in _context.TblSubjectMasters on stu.TradeId equals sub.TradeId
                                      where stu.StudentUserId == student.StudentUserId && sub.TradeId == tradeID
                                      select new StudentTradeDepartmentDTO
                                      {
                                          DepartmentId = stu.DepartmentId ?? 0,
                                          TradeId = stu.TradeId ?? 0,
                                          SubjectName = sub.SubjectName
                                      }
                                 ).ToListAsync();

                return subjects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving subjects by student trade");
                return new List<StudentTradeDepartmentDTO>();
            }


        }

        public async Task AddUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster)
        {
            try
            {
                await _context.TblUserSubscribeMaster.AddAsync(usersubscribemaster);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task AddUserSubjectActivationHistoryAsync(TblUserSubjectActivationHistory usersubjectactivationhistory)
        {
            try
            {
                await _context.TblUserSubjectActivationHistory.AddAsync(usersubjectactivationhistory);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task<bool> UpdateUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster)
        {
            try
            {
                _context.TblUserSubscribeMaster.Update(usersubscribemaster);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }

        public async Task<List<UserSubscriptionDetailDto>> GetUserSubscribeMasterAsync()
        {
            var Query = from u in _context.TblUserSubscribeMaster
                        join h in _context.TblUserSubjectActivationHistory on u.UserSubscribeMasterId equals h.TusmId
                        orderby u.UserSubscribeMasterId descending
                        select new UserSubscriptionDetailDto
                        {
                            UserSubscribeMaster = u,
                            UserSubjectActivationHistory = h
                        };
            return Query.ToList();
        }

        public async Task DeleteUserSubjectActivationHistoryAsync(int Id)
        {
            var getActivationHistory = _context.TblUserSubjectActivationHistory.Where(x => x.TusmId == Id);
            _context.TblUserSubjectActivationHistory.RemoveRange(getActivationHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<TblSubjectMaster> GetPaymentSubject(string subjectCode)
        {
            try
            {
                return await _context.TblSubjectMasters.FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;

            }
        }
    }
}
