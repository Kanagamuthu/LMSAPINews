using LmsAPI.Models;
using LMSAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LMSAPI.Repository
{
    public class StudentsRepository : IStudentsRepository
    {
        private readonly LmsdbNewContext _context;

        public StudentsRepository(LmsdbNewContext context)
        {
            _context = context;
        }
        public async Task<TblStudentUserMaster?> GetStudentByEmailAsync(string email)
        {
            try
            {
                return await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == email);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;

            }
        }
        public async Task AddStudentAsync(TblStudentUserMaster student)
        {
            try
            {
                await _context.TblStudentUserMasters.AddAsync(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        #region OTP function
        public async Task SaveOtpAsync(TblUserRandomPass otpRecord)
        {
            try
            {
                await _context.TblUserRandomPasses.AddAsync(otpRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        #endregion
        public async Task<int> GetTrialPeriodDaysAsync()
        {
            try
            {
                var setting = await _context.TblAppConfigs.FirstOrDefaultAsync(s => s.ConfigKey == "trial_period_days");
                if (setting != null && int.TryParse(setting.ConfigValue, out int days))
                {
                    return days;
                }
                return 0; // Default to 0 if not found or invalid
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task<TblUserRandomPass?> GetLatestOtpAsync(int userId, int actionType, int userType)
        {
            try
            {
                return await _context.TblUserRandomPasses
                    .Where(o => o.UserId == userId && o.ActionType == actionType && o.UserType == userType)
                    .OrderByDescending(o => o.GeneratedTime)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task<bool> UpdateStudentAsync(TblStudentUserMaster student)
        {
            try
            {
                _context.TblStudentUserMasters.Update(student);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }
        public async Task<bool> DeleteOtpAsync(int userId)
        {
            try
            {
                var otpRecords = await _context.TblUserRandomPasses.Where(o => o.UserId == userId).ToListAsync();
                if (otpRecords.Any())
                {
                    _context.TblUserRandomPasses.RemoveRange(otpRecords);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return false;
            }
        }

        public async Task<bool> GetStudentTokenAsync(string token)
        {
            try
            {
                return await _context.TblStudentUserMasters.AnyAsync(x => x.Token == token);
            }
            catch
            {
                return false;
            }
        }

        public async Task TicketCreateAsync(TblSupportTicket request)
        {
            try
            {
                await _context.TblSupportTickets.AddAsync(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        public async Task<List<TblSupportTicket>> GetTicketByIdAsync(int id)
        {

            return await _context.TblSupportTickets.Where(x => x.ReadBy == id).ToListAsync() ?? new List<TblSupportTicket>();

        }

        public async Task RegenerateOtpAsync(TblUserRandomPass otpRecord)
        {
            try
            {
                _context.TblUserRandomPasses.Update(otpRecord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        #region get country

        public async Task<List<TblCountriesCode>> GetCountriesCodesAsync()
        {
            try
            {
                return await _context.TblCountriesCodes.ToListAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        #endregion
    }
}
