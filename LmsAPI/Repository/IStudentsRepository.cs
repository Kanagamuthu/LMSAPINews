using LMSAPI.Models;

namespace LMSAPI.Repository
{
    public interface IStudentsRepository
    {
        Task<TblStudentUserMaster?> GetStudentByEmailAsync(string email);
        Task AddStudentAsync(TblStudentUserMaster student);
        Task<bool> SaveOtpAsync(TblUserRandomPass otpRecord);
        //GEt GetLatestOtpAsync
        Task<TblUserRandomPass?> GetLatestOtpAsync(int userId, int actionType, int userType);
        //update student status
        Task<bool> UpdateStudentAsync(TblStudentUserMaster student);
        //delete otp
        Task<bool> DeleteOtpAsync(int userId);
        //get trail period days
        Task<int> GetTrialPeriodDaysAsync();
        Task<bool> GetStudentTokenAsync(string token);

        Task TicketCreateAsync(TblSupportTicket request);
        Task<List<TblSupportTicket>> GetTicketByIdAsync(int id);

        //re-generate otp
        Task RegenerateOtpAsync(TblUserRandomPass otpRecord);

        Task<List<TblCountriesCode>> GetCountriesCodesAsync();

        //multi device logout
        Task<bool> ValidDeviceAsync(string email, string device_mac);

        //validate OTP for student
        Task<bool> ValidateOtpAsync(int userId, string otp);
    }
}
