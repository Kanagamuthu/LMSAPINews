using LMSAPI.DTO;
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

            return await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == email);

        }
        public async Task AddStudentAsync(TblStudentUserMaster student)
        {

            await _context.TblStudentUserMasters.AddAsync(student);
            await _context.SaveChangesAsync();

        }
        #region OTP function
        public async Task<bool> SaveOtpAsync(TblUserRandomPass otpRecord, int userId)
        {
            var existingOtp = await _context.TblUserRandomPasses.Where(o => o.UserId == userId).OrderByDescending(o => o.GeneratedTime).FirstOrDefaultAsync();
            if (existingOtp == null)
            {
                await _context.TblUserRandomPasses.AddAsync(otpRecord);
            }
            else
            {
                existingOtp.VerificationCode = otpRecord.VerificationCode;
                existingOtp.GeneratedTime = otpRecord.GeneratedTime;
                existingOtp.ActionType = otpRecord.ActionType;
                existingOtp.UserType = otpRecord.UserType;
                _context.TblUserRandomPasses.Update(existingOtp);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        //public async Task<bool> SaveOtpAsync(TblUserRandomPass otpRecord,int userId)
        //{
        //    var otpRecords = await _context.TblUserRandomPasses.OrderByDescending(x => x.GeneratedTime).Where(o => o.UserId == userId).FirstOrDefaultAsync();
        //    if (otpRecords == null)
        //    {
        //        await _context.TblUserRandomPasses.AddAsync(otpRecord);
        //        int result = await _context.SaveChangesAsync();
        //        return result > 0;
        //    }


        //    //try
        //    //{
        //    //    await _context.TblUserRandomPasses.AddAsync(otpRecord);
        //    //    await _context.SaveChangesAsync();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    string message = ex.Message;
        //    //    throw;
        //    //}
        //}
        #endregion
        public async Task<int> GetTrialPeriodDaysAsync()
        {

            var setting = await _context.TblAppConfigs.FirstOrDefaultAsync(s => s.ConfigKey == "trial_period_days");
            if (setting != null && int.TryParse(setting.ConfigValue, out int days))
            {
                return days;
            }
            return 0; // Default to 0 if not found or invalid

        }
        public async Task<TblUserRandomPass?> GetLatestOtpAsync(int userId, int actionType, int userType)
        {

            return await _context.TblUserRandomPasses
                .Where(o => o.UserId == userId && o.ActionType == actionType && o.UserType == userType)
                .OrderByDescending(o => o.GeneratedTime)
                .FirstOrDefaultAsync();

        }
        public async Task<bool> UpdateStudentAsync(TblStudentUserMaster student)
        {

            _context.TblStudentUserMasters.Update(student);
            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<bool> UpdateOtpAsync(int userId)
        {

            var otpRecords = await _context.TblUserRandomPasses.OrderByDescending(x => x.GeneratedTime).Where(o => o.UserId == userId).FirstOrDefaultAsync();
            if (otpRecords != null)
            {
                _context.TblUserRandomPasses.Update(otpRecords);
                await _context.SaveChangesAsync();
            }

            return true;

        }
        public async Task<bool> GetStudentTokenAsync(string token)
        {

            return await _context.TblStudentUserMasters.AnyAsync(x => x.Token == token);

        }

        public async Task TicketCreateAsync(TblSupportTicket request)
        {

            await _context.TblSupportTickets.AddAsync(request);
            await _context.SaveChangesAsync();

        }

        public async Task<List<SupportTicketDto>> GetTicketByEmailAsync(string email)
        {
            var tickets = await _context.TblSupportTickets
                                        .Where(x => x.EmailId == email)
                                        .ToListAsync();

            var ticketDtos = tickets.Select(ticket => new SupportTicketDto
            {
                ticketId = ticket.StId,
                subject = ticket.Subject,
                message = ticket.Message,
                resolution = ticket.Resolution,
                CreatedAt = ticket.Createdon ?? DateTime.MinValue,  // safer for nullable DateTime
                Status = ticket.ActiveStatus ?? false  // safer for nullable bool
            }).ToList();

            return ticketDtos;
        }

        //public async Task RegenerateOtpAsync(TblUserRandomPass otpRecord,int userId)
        public async Task<bool> RegenerateOtpAsync(TblUserRandomPass otpRecord, int userId)
        {

            var existingOtp = await _context.TblUserRandomPasses
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.GeneratedTime)
                .FirstOrDefaultAsync();

            if (existingOtp != null)
            {
                // UPDATE existing record
                existingOtp.VerificationCode = otpRecord.VerificationCode;
                existingOtp.GeneratedTime = otpRecord.GeneratedTime;
                existingOtp.ActionType = otpRecord.ActionType;
                existingOtp.UserType = otpRecord.UserType;

                _context.TblUserRandomPasses.Update(existingOtp);
            }

            await _context.SaveChangesAsync();
            return true;

        }


        #region get country
        public async Task<List<TblCountriesCode>> GetCountriesCodesAsync()
        {

            return await _context.TblCountriesCodes.ToListAsync();

        }

        #endregion
        public async Task<bool> ValidDeviceAsync(string email, string deviceMac)
        {

            return await _context.TblStudentUserMasters.AnyAsync(s => s.EmailId == email && s.PrimaryMac == deviceMac);

        }

        public async Task<bool> ValidateOtpAsync(int userId, string otp)
        {

            return await _context.TblUserRandomPasses.AnyAsync(o => o.UserId == userId && o.VerificationCode == otp);

        }

        //upate otp with userid & data
        public async Task<bool> UpdateOtpAsyncnew(TblUserRandomPass otpRecord, int userId)
        {

            var otpRecords = await _context.TblUserRandomPasses.OrderByDescending(x => x.GeneratedTime).Where(o => o.UserId == userId).FirstOrDefaultAsync();
            if (otpRecords != null)
            {
                //_context.TblUserRandomPasses.Update(otpRecord);
                otpRecords.VerificationCode = otpRecord.VerificationCode;
                otpRecords.GeneratedTime = otpRecord.GeneratedTime;
                otpRecords.ActionType = otpRecord.ActionType;
                otpRecords.UserType = otpRecord.UserType;
                await _context.SaveChangesAsync();
            }

            return true;

        }
    }
}
