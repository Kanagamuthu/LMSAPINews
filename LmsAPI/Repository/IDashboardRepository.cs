using LMSAPI.DTO;
using LMSAPI.Models;

namespace LMSAPI.Repository
{
    public interface IDashboardRepository
    {
        bool IsValidStudent(string userEmail);

        //list all subjects
        Task<List<TblSubjectMaster>> GetAllSubjects();

        //add books to student
        Task<bool> AddBooksToStudent(string userEmail, List<int> bookIds);

        //get limit of books per student
        Task<int> GetBookLimitPerStudentAsync();

        Task<int> GetCurrentBookCountForStudentAsync(int sud);

        //find Trades SetInactive
        Task SetInactive(long sud);
        //Task GetActiveTradesByUserIDAsync(long sud);
        Task<List<TblStudentTrialSubject>> GetActiveTradesByUserIDAsync(long userId);
        Task<bool> PostRegisterStudentTradeDepartment(string userEmail, StudentTradeDepartmentDTO studentTradeDepartmentDTO);
        Task<List<StudentTradeDepartmentDTO>> GetSubjectsByStudentTrade(string userEmail, int tradeID);

        Task AddUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster);
        Task AddUserSubjectActivationHistoryAsync(TblUserSubjectActivationHistory usersubjectactivationhistory);
        Task<bool> UpdateUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster);
        Task<List<UserSubscriptionDetailDto>> GetUserSubscribeMasterAsync();
        Task DeleteUserSubjectActivationHistoryAsync(int Id);
        Task<TblSubjectMaster> GetPaymentSubject(string subjectCode);
        Task<List<DepartmentSubjectDTO>> GetAllDepartmentSubjects();

        Task AddReadHistoryAsync(TblReadHistory obj);
        Task<ReadHistoryDto> ReadHistory(int Id);
    }
}
