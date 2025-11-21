
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
        private readonly LmsdbNewContext _context;
        private readonly ILoggerManager _logger;
        private readonly IStudentsRepository _studentsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DashboardRepository(LmsdbNewContext context, ILoggerManager logger, IStudentsRepository studentsRepository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _studentsRepository = studentsRepository;
            _httpContextAccessor = httpContextAccessor;
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
        public async Task<bool> Ad_contextooksToStudent(string userEmail, List<int> bookIds)
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
        public async Task<TblStudentUserMaster> PostRegisterStudentTradeDepartment(string userEmail, StudentTradeDepartmentDTO studentTradeDepartmentDTO)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == userEmail);

                // update the department
                student.DepartmentName = studentTradeDepartmentDTO.department_name;
                student.Collegename = studentTradeDepartmentDTO.collegename;
                student.EduType = studentTradeDepartmentDTO.edutype;
                student.Batchyear = studentTradeDepartmentDTO.batchyear;
                //student.TradeId = studentTradeDepartmentDTO.TradeId;

                // mark entity as modified and save

                _context.TblStudentUserMasters.Update(student);
                await _context.SaveChangesAsync();
                return student;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering student trade and department");
                return new TblStudentUserMaster();
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
                                      where stu.StudentUserId == student.StudentUserId
                                      select new StudentTradeDepartmentDTO
                                      {
                                          edutype = stu.EduType,
                                          department_name = stu.DepartmentName,
                                          batchyear = stu.Batchyear,
                                          collegename = stu.Collegename,
                                          //SubjectName = sub.SubjectName
                                      }
                                      //where stu.StudentUserId == student.StudentUserId && sub.TradeId == tradeID
                                      //select new StudentTradeDepartmentDTO
                                      //{
                                      //    DepartmentId = stu.DepartmentId ?? 0,
                                      //    TradeId = stu.TradeId ?? 0,
                                      //    SubjectName = sub.SubjectName
                                      //}
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
                await _context.TblUserSubscribeMasters.AddAsync(usersubscribemaster);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task AddUserSubjectActivationHistoryAsync(List<TblUserSubjectActivationHistory> usersubjectactivationhistory)
        {
            try
            {
                await _context.TblUserSubjectActivationHistories.AddRangeAsync(usersubjectactivationhistory);
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
                _context.TblUserSubscribeMasters.Update(usersubscribemaster);
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
            var Query = from u in _context.TblUserSubscribeMasters
                            //join h in _context.TblUserSubjectActivationHistories on u.UserSubscribeMasterId equals h.TusmId
                        orderby u.UserSubscribeMasterId descending
                        select new UserSubscriptionDetailDto
                        {
                            UserSubscribeMaster = u,
                            //UserSubjectActivationHistory = h
                        };
            return Query.ToList();
        }

        public async Task DeleteUserSubjectActivationHistoryAsync(int Id)
        {
            var getActivationHistory = _context.TblUserSubjectActivationHistories.Where(x => x.TusmId == Id);
            _context.TblUserSubjectActivationHistories.RemoveRange(getActivationHistory);
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

        public async Task<List<DepartmentSubjectDTO>> GetAllDepartmentSubjects()
        {
            try
            {
                var query = (from um in _context.TblStudentUserMasters
                             join dm in _context.TblDepartmentMasters on um.DepartmentId equals dm.DepartmentId
                             join dsm in _context.TblDepartmentSubjectMappings on um.DepartmentId equals dsm.DepartmentId
                             join sm in _context.TblSubjectMasters on dsm.SubjectId equals sm.SubjectId
                             select new DepartmentSubjectDTO
                             {
                                 DepartmentId = dm.DepartmentId,
                                 DepartmentName = dm.DepartmentName,
                                 subjectMaster = new SubjectMasterDto(_httpContextAccessor, this, _context)
                                 {
                                     SubjectId = sm.SubjectId,
                                     SubjectCode = sm.SubjectCode,
                                     UnivSubjectCode = sm.UnivSubjectCode,
                                     SubjectName = sm.SubjectName,
                                     SubjectCoverPath = sm.SubjectCoverPath,
                                     SubjectDescription = sm.SubjectDescription,
                                     ActiveStatus = sm.ActiveStatus,
                                     RuleId = sm.RuleId,
                                     CreatedOn = sm.CreatedOn,
                                     ReleasedOn = sm.ReleasedOn,
                                     UniversityId = sm.UniversityId,
                                     HavingQuestionpaper = sm.HavingQuestionpaper,
                                     SubjectVersion = sm.SubjectVersion,
                                     ActiveDurationDays = sm.ActiveDurationDays,
                                     ActiveDurationDate = sm.ActiveDurationDate,
                                     Syllabus = sm.Syllabus,
                                     DeptImgPath = sm.DeptImgPath,
                                     Coursehours = sm.Coursehours,
                                     Visuals = sm.Visuals,
                                     Pagecontent = sm.Pagecontent ?? 0,
                                     Solvedproblem = sm.Solvedproblem,
                                     Multichoice = sm.Multichoice,
                                     DeptVideo = sm.DeptVideo,
                                     IsInTrail = sm.IsInTrail,
                                     IsInDemo = sm.IsInDemo,
                                     TradeId = sm.TradeId,
                                     SubjectSyllabusPath = sm.SubjectSyllabusPath
                                 }

                             }).Distinct().ToList();

                return query;

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }

        }
        public async Task AddReadHistoryAsync(TblReadHistory obj)
        {
            try
            {
                await _context.TblReadHistories.AddAsync(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }
        public async Task<bool> GetReadHistory(TblReadHistory obj)
        {
            var exists = await _context.TblReadHistories.AnyAsync(rh => rh.SubjctCode == obj.SubjctCode && rh.Url == obj.Url && rh.Type == obj.Type && rh.Readby == obj.Readby);
            return exists;
        }

        public List<TblReadHistory> GetAllReadHistory()
        {
            var data = _context.TblReadHistories.ToList();
            return data;
        }

        public async Task<ReadHistoryDto> ReadHistory(int Id)
        {
            try
            {
                var histories = await _context.TblReadHistories.Where(rh => rh.Readby == Id && rh.Status == true).ToListAsync();
                var Getpurchase = from sm in _context.TblSubjectMasters
                                  join h in _context.TblUserSubjectActivationHistories on Convert.ToInt32(sm.SubjectId) equals h.SubjectId
                                  where h.UserId == Id && h.SubjectExpiryDate.Value.Date >= DateTime.Now.Date
                                  select sm;
                var getbook = Getpurchase.Distinct().ToList();

                var videoCount = histories.Count(rh => rh.Type.ToLower() == "video");
                var pageCount = histories.Count(rh => rh.Type.ToLower() != "video" && rh.Type.ToLower() != "bookmark" && rh.Type.ToLower() != "download");

                var download = histories.Where(rh => rh.Type.ToLower() == "download").ToList();
                var bookmark = histories.Where(rh => rh.Type.ToLower() == "bookmark").ToList();

                return new ReadHistoryDto
                {
                    VideoCount = videoCount,
                    PageCount = pageCount,
                    Bookmarks = bookmark,
                    Downloads = download,
                    Purchase = getbook
                };
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        public async Task<List<TblPackageMaster>> GetAllPackage()
        {
            var Query = from p in _context.TblPackageMasters where p.Activestatus == true select p;
            return await Query.OrderByDescending(x => x.CreatedOn).ToListAsync();
        }
        public async Task<List<PackageDetailsDTO>> GetPackageDetails(int PackageId, int userId)
        {
            DateTime now = DateTime.Now;

            // Single merged left-join query
            var data = await (
                from pm in _context.TblPackageMasters
                join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
                join sm in _context.TblSubjectMasters on pd.SubjectId equals sm.SubjectId
                join d in _context.TblDepartmentMasters on pd.DepartmentId equals d.DepartmentId

                // LEFT JOIN: User purchase history
                join usm in _context.TblUserSubscribeMasters
                    on pm.PackageId equals usm.PackageId into usmGroup
                from usm in usmGroup.Where(x => x.UserId == userId && x.PaymentStatus == "Success").DefaultIfEmpty()
                    // LEFT JOIN: Activation history
                join sah in _context.TblUserSubjectActivationHistories
                    on (usm != null ? usm.UserSubscribeMasterId : 0) equals sah.TusmId into sahGroup
                from sah in sahGroup.DefaultIfEmpty()
                where pm.PackageId == PackageId && pm.Activestatus == true
                select new
                {
                    pd.DepartmentId,
                    d.DepartmentName,
                    pd.PackageId,
                    pm.PackageName,
                    pm.SellingPrice,
                    pm.CoverPath,
                    pm.PackageDurationDays,
                    // Subject
                    sm,

                    // User data
                    PaymentOn = usm.PaymentOn,
                    SubjectExpiryDate = sah.SubjectExpiryDate
                }
            )
            .ToListAsync();


            // Compute validity
            int validityDays = data.Max(x => x.PackageDurationDays ?? 0);
            DateTime expiryDate = now.AddDays(validityDays);
            // Group by Package (pack all subjects inside)
            var result = data
                .GroupBy(x => new{ x.DepartmentId,x.DepartmentName,x.PackageId,x.PackageName,x.SellingPrice,x.CoverPath,x.PackageDurationDays})
                .Select(pkg => new PackageDetailsDTO
                {
                    DepartmentId = pkg.Key.DepartmentId,
                    DepartmentName = pkg.Key.DepartmentName,
                    PackageId = pkg.Key.PackageId,
                    PackageName = pkg.Key.PackageName,
                    Coverpath = pkg.Key.CoverPath,
                    Price = pkg.Key.SellingPrice,
                    Validity = pkg.Key.PackageDurationDays + " Days",
                    Validitydate = expiryDate.ToString("yyyy-MM-dd"),
                    // ✔ Using Max() ensures correct values
                    PaymentOn = pkg.Max(x => x.PaymentOn),
                    SubjectExpiryDate = pkg.Max(x => x.SubjectExpiryDate),
                    // ✔ Correct isPurchased logic
                    IsPurchased = pkg.Max(x => x.PaymentOn) != null,
                    // Pack all subjects
                    SubjectMaster = pkg.Select(x => new SubjectMasterDto(_httpContextAccessor, this, _context)
                    {
                        SubjectId = x.sm.SubjectId,
                        SubjectCode = x.sm.SubjectCode,
                        SubjectName = x.sm.SubjectName,
                        SubjectCoverPath = x.sm.SubjectCoverPath,
                        SubjectDescription = x.sm.SubjectDescription,
                        ActiveStatus = x.sm.ActiveStatus,
                        RuleId = x.sm.RuleId,
                        CreatedOn = x.sm.CreatedOn,
                        ReleasedOn = x.sm.ReleasedOn,
                        UniversityId = x.sm.UniversityId,
                        HavingQuestionpaper = x.sm.HavingQuestionpaper,
                        SubjectVersion = x.sm.SubjectVersion,
                        ActiveDurationDays = x.sm.ActiveDurationDays,
                        ActiveDurationDate = x.sm.ActiveDurationDate,
                        Syllabus = x.sm.Syllabus,
                        DeptImgPath = x.sm.DeptImgPath,
                        Coursehours = x.sm.Coursehours,
                        Visuals = x.sm.Visuals,
                        Pagecontent = x.sm.Pagecontent ?? 0,
                        Solvedproblem = x.sm.Solvedproblem,
                        Multichoice = x.sm.Multichoice,
                        DeptVideo = x.sm.DeptVideo,
                        IsInTrail = x.sm.IsInTrail,
                        IsInDemo = x.sm.IsInDemo,
                        TradeId = x.sm.TradeId,
                        SubjectSyllabusPath = x.sm.SubjectSyllabusPath
                    }).ToList()
                })
                .ToList();

            return result;
        }

        //public async Task<List<PackageDetailsDTO>> GetPackageDetails(int PackageId, int userId)
        //{

        //    var data = await (from pm in _context.TblPackageMasters
        //                join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
        //                join sm in _context.TblSubjectMasters on pd.SubjectId equals sm.SubjectId
        //                join d in _context.TblDepartmentMasters on pd.DepartmentId equals d.DepartmentId
        //                join usm in _context.TblUserSubscribeMasters
        //                on pm.PackageId equals usm.PackageId into usmGroup
        //                from usm in usmGroup .Where(x => x.UserId == userId && x.PaymentStatus == "Success").DefaultIfEmpty()
        //                join sah in _context.TblUserSubjectActivationHistories
        //                    on usm.UserSubscribeMasterId equals sah.TusmId into sahGroup
        //                from sah in sahGroup.DefaultIfEmpty()
        //                where pm.PackageId == PackageId && pm.Activestatus == true
        //                select new
        //                {
        //                    pd.DepartmentId,
        //                    d.DepartmentName,
        //                    pd.PackageDetailId,
        //                    pd.PackageId,
        //                    pm.PackageName,
        //                    pm.SellingPrice,
        //                    pm.CoverPath,
        //                    pm.PackageDurationDays,
        //                    sm,
        //                    PaymentOn = usm != null ? usm.PaymentOn : null,
        //                    SubjectExpiryDate = sah != null ? sah.SubjectExpiryDate : null
        //                }).ToListAsync();

        //                 //Validity calculation
        //                 int validityDays = data.Max(x => x.PackageDurationDays ?? 0);
        //                 //current date+validity days
        //                 DateTime currentDate = DateTime.Now;
        //                 DateTime expiryDate = currentDate.AddDays(validityDays);
        //                 //map to is purchase details dto
        //    var result = data.Select(x => new PackageDetailsDTO
        //    {
        //        DepartmentId = x.DepartmentId,
        //        DepartmentName = x.DepartmentName,
        //        PackageId = x.PackageId,
        //        PackageName = x.PackageName,
        //        Coverpath = x.CoverPath,
        //        Validity = validityDays + " Days",
        //        Validitydate = expiryDate.ToString("yyyy-MM-dd"),
        //        Price = x.SellingPrice,
        //        IsPurchased = x.PaymentOn !=null?true:false,
        //        PaymentOn = x.PaymentOn,
        //        SubjectExpiryDate = x.SubjectExpiryDate,
        //        SubjectMaster = new List<SubjectMasterDto>
        //        {
        //             new SubjectMasterDto(_httpContextAccessor, this, _context)
        //             {
        //                 SubjectId = x.sm.SubjectId,
        //                 SubjectCode = x.sm.SubjectCode,
        //                 SubjectName = x.sm.SubjectName,
        //                 SubjectCoverPath = x.sm.SubjectCoverPath,
        //                 SubjectDescription = x.sm.SubjectDescription,
        //                 ActiveStatus = x.sm.ActiveStatus,
        //                 RuleId = x.sm.RuleId,
        //                 CreatedOn = x.sm.CreatedOn,
        //                 ReleasedOn = x.sm.ReleasedOn,
        //                 UniversityId = x.sm.UniversityId,
        //                 HavingQuestionpaper = x.sm.HavingQuestionpaper,
        //                 SubjectVersion = x.sm.SubjectVersion,
        //                 ActiveDurationDays = x.sm.ActiveDurationDays,
        //                 ActiveDurationDate = x.sm.ActiveDurationDate,
        //                 Syllabus = x.sm.Syllabus,
        //                 DeptImgPath = x.sm.DeptImgPath,
        //                 Coursehours = x.sm.Coursehours,
        //                 Visuals = x.sm.Visuals,
        //                 Pagecontent = x.sm.Pagecontent ?? 0,
        //                 Solvedproblem = x.sm.Solvedproblem,
        //                 Multichoice = x.sm.Multichoice,
        //                 DeptVideo = x.sm.DeptVideo,
        //                 IsInTrail = x.sm.IsInTrail,
        //                 IsInDemo = x.sm.IsInDemo,
        //                 TradeId = x.sm.TradeId,
        //                 SubjectSyllabusPath = x.sm.SubjectSyllabusPath,

        //             }
        //        }
        //    }).Distinct().ToList();

        //    return result;
        //}

        //04/11/2025
        public async Task<List<TblDegreeMaster>> GetAllDegrees()
        {
            try
            {
                return await _context.TblDegreeMasters.Where(d => d.IsActive == 1).ToListAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }

        public async Task<List<TblUserSubjectActivationHistory>> AddSubjectToStudent(string userEmail, List<int> subjectId)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == userEmail);
                if (student == null)
                {
                    return new List<TblUserSubjectActivationHistory>();
                }
                //get subject details
                var subjects = await _context.TblSubjectMasters.Where(s => subjectId.Contains((int)s.SubjectId)).ToListAsync();
                if (subjects == null || subjects.Count == 0)
                {
                    return new List<TblUserSubjectActivationHistory>();
                }
                else
                {
                    //save
                    var activationHistories = new List<TblUserSubjectActivationHistory>();
                    foreach (var subject in subjects)
                    {
                        var activationHistory = new TblUserSubjectActivationHistory
                        {
                            SubjectId = (int)subject.SubjectId,
                            UserId = (int)student.StudentUserId,
                            SubjectCode = subject.SubjectCode,
                            SubjectName = subject.SubjectName,
                            SubjectVersion = subject.SubjectVersion,
                            DepartmentId = student.DepartmentId,
                            TusmId = (int)subject.SubjectId,
                            ActivatedOn = DateTime.UtcNow,
                            ActivatedBy = (int)student.StudentUserId,
                            ActivationType = 1, // assuming 1 indicates manual activation
                            ActivationProductType = 1 // assuming 1 indicates standard product type
                        };
                        activationHistories.Add(activationHistory);
                    }
                    await _context.TblUserSubjectActivationHistories.AddRangeAsync(activationHistories);
                }
                await _context.SaveChangesAsync();
                return await _context.TblUserSubjectActivationHistories
                    .Where(ush => ush.UserId == student.StudentUserId && subjectId.Contains((int)ush.SubjectId)).ToListAsync();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                throw;
            }
        }


        public async Task<List<PaymentPackageDTO>> GetpaymentPackage(int packageId)
        {
            var result = await (from pd in _context.TblPackageDetails
                                join tpm in _context.TblPackageMasters on pd.PackageId equals tpm.PackageId
                                join dsm in _context.TblDepartmentSubjectMappings on pd.DepartmentSubjectMappingId equals dsm.DepartmentSubjectMappingId
                                join sm in _context.TblSubjectMasters on dsm.SubjectId equals sm.SubjectId
                                where tpm.PackageId == packageId
                                group new { pd, dsm, sm } by tpm into g
                                select new PaymentPackageDTO
                                {
                                    packagemaster = g.Key,
                                    packagedetails = g.Select(x => x.pd).ToList(),
                                    departmentsubjectmapping = g.Select(x => x.dsm).ToList(),
                                    subjectmaster = g.Select(x => x.sm).ToList()
                                }).ToListAsync();

            return result;
        }
        public async Task<DateTime?> GetActiveOnDateByUserId(long userId)
        {

            var activeDate = await _context.TblStudentUserMasters
                .Where(x => x.StudentUserId == userId)
                .Select(x => x.AccActiveOn)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();

            return activeDate;
        }

        public async Task<int> GetTrialPeriodDaysAsync()
        {
            var value = await _context.TblAppConfigs
                .Where(x => x.ConfigKey == "trial_period_days")
                .Select(x => x.ConfigValue)
                .FirstOrDefaultAsync();

            int trialDays;
            if (!int.TryParse(value, out trialDays))
                trialDays = 0;

            return trialDays;
        }
        public async Task<List<DepartmentMasterDto>> GetRegisterDropdwonList()
        {
            var getDepartmentList = await _context.TblDepartmentMasters.Where(d => d.IsActive == 1).ToListAsync();

            return getDepartmentList.Select(item => new DepartmentMasterDto
            {
                Id = item.DepartmentId,
                Department_name = item.DepartmentName
            }).ToList();
        }

        public async Task<List<EducationListDto>> GetEducationTypeListAsync()
        {
            try
            {
                var educationTypes = await _context.TblEducationTypes.Where(et => et.IsActive == 1).ToListAsync();
                var degreesDto = new List<EducationListDto>();
                foreach (var item in educationTypes)
                {
                    degreesDto.Add(new EducationListDto
                    {
                        id = item.EduId,
                        degree_name = item.EduDes
                    });
                }
                return degreesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving education type list.");
                throw;
            }
        }

        public async Task<List<TblStudentUserMaster>> GetAllPackageByUserEmailAsync(string email)
        {
            try
            {
                var student = await _context.TblStudentUserMasters.FirstOrDefaultAsync(s => s.EmailId == email);
                if (student == null)
                {
                    return new List<TblStudentUserMaster>();
                }
                var packages = await _context.TblStudentUserMasters.Where(s => s.StudentUserId == student.StudentUserId).ToListAsync();
                return packages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving packages by user email.");
                return new List<TblStudentUserMaster>();
            }
        }

        public async Task<List<DepartmentMasterDto>> GetDepartmentByEduTypeIdAsync(int eduTypeId)
        {
            try
            {
                var departments = await _context.TblDepartmentMasters
                    .Where(d => d.DegreeId == eduTypeId && d.IsActive == 1)
                    .ToListAsync();
                return departments.Select(item => new DepartmentMasterDto
                {
                    Id = item.DepartmentId,
                    Department_name = item.DepartmentName,
                    Department_description = item.DepartmentName,
                    Edu_type_id = item.DegreeId
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving departments by education type ID.");
                return new List<DepartmentMasterDto>();
            }
        }

        public async Task<List<StudentPackageDetailsDTO>> GetPackageDetailsByUserEmailAsync(int DegreeId)
        {
            try
            {
                var result = (from pm in _context.TblPackageMasters
                              join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
                              join dm in _context.TblDepartmentMasters on pd.DepartmentId equals dm.DepartmentId
                              where dm.DegreeId == DegreeId && pm.Activestatus == true
                              select new
                              {
                                  dm.DepartmentName,
                                  pm
                              })
                              .AsEnumerable()
                              .GroupBy(x => x.DepartmentName)
                              .Select(g => new StudentPackageDetailsDTO
                              {
                                  DepartmentName = g.Key,
                                  packageMasterDto = g.Select(x => new PackageMasterDto
                                  {
                                      PackageId = x.pm.PackageId,
                                      PackageCode = x.pm.PackageCode,
                                      PackageName = x.pm.PackageDisplayName,
                                      SellingPrice = x.pm.SellingPrice,
                                      CoverPath = x.pm.CoverPath,

                                  }).ToList()
                              })
                              .ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving package details by user.");
                return new List<StudentPackageDetailsDTO>();
            }

        }

        public async Task<List<StudentpurchaseitemsDto>> GetUserPurchaseExpiryAsync(long userId)
        {
            //var result = await (from usm in _context.TblUserSubscribeMasters
            //                    join ah in _context.TblUserSubjectActivationHistories
            //                        on usm.UserSubscribeMasterId equals ah.TusmId
            //                    where usm.PaymentStatus == "success"
            //                          && usm.UserId == userId
            //                    group new { usm, ah } by ah.SubjectCode into g
            //                    orderby g.Max(x => x.usm.PaymentOn) descending
            //                    select new StudentpurchaseitemsDto
            //                    {
            //                        SubjectCode = g.Key,
            //                        SubjectExpiryDate = g.Max(x => x.ah.SubjectExpiryDate),
            //                        PaymentOn = g.Max(x => x.usm.PaymentOn)
            //                    })
            //                    .ToListAsync();

            var result = await (from usm in _context.TblUserSubscribeMasters
                                join ah in _context.TblUserSubjectActivationHistories on usm.UserSubscribeMasterId equals ah.TusmId
                                join pm in _context.TblPackageMasters on usm.PackageId equals pm.PackageId
                                where usm.PaymentStatus == "success" && usm.UserId == userId
                                group new { usm, ah, pm } by new
                                {
                                    pm.PackageId,
                                    pm.PackageCode,
                                    pm.PackageName,
                                    pm.SellingPrice,
                                    pm.CoverPath,
                                    ah.SubjectExpiryDate,
                                    usm.PaymentOn,
                                    
                                } into g
                                orderby g.Max(x => x.usm.PaymentOn) descending
                             select new StudentpurchaseitemsDto
                             {
                                 PackageId = g.Key.PackageId,
                                 PackageCode =g.Key.PackageCode,
                                 PackageName = g.Key.PackageName,
                                 SellingPrice = g.Key.SellingPrice,
                                 CoverPath = g.Key.CoverPath,
                                 
                                 SubjectExpiryDate = g.Max(x => x.ah.SubjectExpiryDate),
                                 PaymentOn = g.Max(x => x.usm.PaymentOn)
                             })
                             .ToListAsync();

            return result;
        }
    }
}
