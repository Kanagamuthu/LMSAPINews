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
        Task<TblStudentUserMaster> PostRegisterStudentTradeDepartment(string userEmail, StudentTradeDepartmentDTO studentTradeDepartmentDTO);
        Task<List<StudentTradeDepartmentDTO>> GetSubjectsByStudentTrade(string userEmail, int tradeID);

        Task AddUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster);
        Task AddUserSubjectActivationHistoryAsync(List<TblUserSubjectActivationHistory> usersubjectactivationhistory);
        Task<bool> UpdateUserSubscribeMasterAsync(TblUserSubscribeMaster usersubscribemaster);
        Task<List<UserSubscriptionDetailDto>> GetUserSubscribeMasterAsync();
        Task DeleteUserSubjectActivationHistoryAsync(int Id);
        Task<TblSubjectMaster> GetPaymentSubject(string subjectCode);
        Task<List<DepartmentSubjectDTO>> GetAllDepartmentSubjects();

        Task AddReadHistoryAsync(TblReadHistory obj);
        Task<bool> GetReadHistory(TblReadHistory obj);
        List<TblReadHistory> GetAllReadHistory();
        Task<ReadHistoryDto> ReadHistory(int Id);

        Task<List<TblPackageMaster>> GetAllPackage();
        //Task<List<PackageDetailsDTO>> GetPackageDetails(string PackageId, int userId);
        Task<PackageDetailsDTO> GetPackageDetails(string packageId, int userId);

        Task<List<TblDegreeMaster>> GetAllDegrees();

        Task<List<TblUserSubjectActivationHistory>> AddSubjectToStudent(string userEmail, List<int> subjectId);

        Task<List<PaymentPackageDTO>> GetpaymentPackage(int packageId);

        Task<DateTime?> GetActiveOnDateByUserId(long userId);
        Task<int> GetTrialPeriodDaysAsync();
        Task<List<DepartmentMasterDto>> GetRegisterDropdwonList();

        Task<List<EducationListDto>> GetEducationTypeListAsync();

        //get department by education type id
        Task<List<DepartmentMasterDto>> GetDepartmentByEduTypeIdAsync(int eduTypeId);

        Task<List<TblStudentUserMaster>> GetAllPackageByUserEmailAsync(string email);
        Task<List<StudentPackageDetailsDTO>> GetPackageDetailsByUserEmailAsync(int DegreeId);

        Task<List<StudentpurchaseitemsDto>> GetUserPurchaseExpiryAsync(long userId);

        Task<int> CreatePackageAsync(CreatePackageDto packageDto, int _userid);
    }
}
