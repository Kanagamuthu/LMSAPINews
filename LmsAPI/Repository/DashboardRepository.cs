
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
                                     SubjectId = sm.SubjectId.ToString(),
                                     SubjectCode = sm.SubjectCode,
                                     UnivSubjectCode = sm.UnivSubjectCode,
                                     SubjectName = sm.SubjectName,
                                     SubjectCoverPath = sm.SubjectCoverPath,
                                     SubjectDescription = sm.SubjectDescription,
                                     ActiveStatus = sm.ActiveStatus.ToString(),
                                     RuleId = sm.RuleId.ToString(),
                                     CreatedOn = sm.CreatedOn ?? DateTime.Now,
                                     ReleasedOn = sm.ReleasedOn,
                                     UniversityId = sm.UniversityId.ToString(),
                                     HavingQuestionpaper = sm.HavingQuestionpaper.ToString(),
                                     SubjectVersion = sm.SubjectVersion,
                                     ActiveDurationDays = sm.ActiveDurationDays.ToString(),
                                     ActiveDurationDate = sm.ActiveDurationDate ?? DateTime.Now,
                                     Syllabus = sm.Syllabus,
                                     DeptImgPath = sm.DeptImgPath,
                                     Coursehours = sm.Coursehours.ToString(),
                                     Visuals = sm.Visuals.ToString(),
                                     Pagecontent = sm.Pagecontent.ToString() ?? "",
                                     Solvedproblem = sm.Solvedproblem.ToString(),
                                     Multichoice = sm.Multichoice.ToString(),
                                     DeptVideo = sm.DeptVideo,
                                     // IsInTrail = sm.IsInTrail,
                                     IsInDemo = sm.IsInDemo.ToString(),
                                     TradeId = sm.TradeId.ToString(),
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
            //var exists = await _context.TblReadHistories.AnyAsync(rh => rh.SubjctCode == obj.SubjctCode && rh.Url == obj.Url && rh.Type == obj.Type && rh.Readby == obj.Readby);
            //return exists;
            return false;
        }

        public List<TblReadHistory> GetAllReadHistory()
        {
            var data = _context.TblReadHistories.ToList();
            return data;
        }

        public async Task<ReadHistoryDto> ReadHistory1(int Id)
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

        public async Task<List<readhistorydto>> ReadHistory(int Id)
        {
            try
            {
                var result = (from pm in _context.TblPackageMasters
                              join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
                              join dm in _context.TblDepartmentMasters on pd.DepartmentId equals dm.DepartmentId
                              join usm in _context.TblUserSubscribeMasters on pm.PackageId equals usm.PackageId 
                              join sah in _context.TblUserSubjectActivationHistories on usm.UserSubscribeMasterId equals sah.TusmId 
                              where pm.Activestatus == true && usm.UserId == Id && usm.PaymentStatus.ToLower() == "success"
                              select new
                              {
                                  dm.DepartmentName,
                                  pm,
                                  sah
                              }).AsEnumerable().GroupBy(x => x.DepartmentName).
                              Select(g => new readhistorydto
                              {
                                  departmentName = g.Key,
                                  packageMasterDto =g.OrderByDescending(x=>x.sah.SubjectExpiryDate).GroupBy(x => x.pm.PackageId)
                                 .Select(p => new Packagemasterdto
                                 {
                                     packageId = p.First().pm.PackageId,
                                     packageCode = p.First().pm.PackageCode,
                                     packageName = p.First().pm.PackageDisplayName,
                                     sellingPrice = p.First().pm.SellingPrice??0,
                                     coverPath = p.First().pm.CoverPath,
                                     isPurchased = true,
                                     subjectExpiryDate =  p.First().sah.SubjectExpiryDate,
                                     paymentOn = p.First().sah.ActivatedOn

                                 }).ToList()
                              }).ToList();


                return result;
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

        //02-12-2025
        //public async Task<List<PackageDetailsDTO>> GetPackageDetails(string packageId, int userId)
        //{
        //    long pkgId = Convert.ToInt64(packageId);

        //    var data = await (from pm in _context.TblPackageMasters
        //                      join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
        //                      join sm in _context.TblSubjectMasters on pd.SubjectId equals sm.SubjectId
        //                      join d in _context.TblDepartmentMasters on pd.DepartmentId equals d.DepartmentId
        //                      join usm in _context.TblUserSubscribeMasters
        //                          on pm.PackageId equals usm.PackageId into usmGroup
        //                      from usm in usmGroup
        //                          .Where(x => x.UserId == userId && x.PaymentStatus == "Success")
        //                          .DefaultIfEmpty()
        //                      join sah in _context.TblUserSubjectActivationHistories
        //                          on usm.UserSubscribeMasterId equals sah.TusmId into sahGroup
        //                      from sah in sahGroup.DefaultIfEmpty()
        //                      where pm.PackageId == pkgId && pm.Activestatus == true
        //                      select new
        //                      {
        //                          pd.DepartmentId,
        //                          d.DepartmentName,
        //                          pd.PackageDetailId,
        //                          pd.PackageId,
        //                          pm.PackageName,
        //                          pm.SellingPrice,
        //                          pm.CoverPath,
        //                          pm.PackageDurationDays,
        //                          Subject = sm,
        //                          PaymentOn = usm != null ? usm.PaymentOn : null,
        //                          SubjectExpiryDate = sah != null ? sah.SubjectExpiryDate : null
        //                      })
        //                      .ToListAsync();

        //    if (data == null || data.Count == 0)
        //        return new List<PackageDetailsDTO>();


        //    // -------------- VALIDITY --------------
        //    int validityDays = data.Max(x => x.PackageDurationDays ?? 0);


        //    // -------------- CREATE SINGLE PACKAGE DTO --------------
        //    var first = data.First();

        //    var package = new PackageDetailsDTO
        //    {
        //        DepartmentId = first.DepartmentId.ToString(),
        //        DepartmentName = first.DepartmentName,
        //        PackageId = first.PackageId.ToString(),
        //        PackageName = first.PackageName,
        //        Coverpath = first.CoverPath,
        //        Validity = validityDays.ToString(),
        //        Validitydate = first.SubjectExpiryDate,
        //        Price = first.SellingPrice.ToString(),
        //        IsPurchased = first.PaymentOn != null || first.SubjectExpiryDate != null,
        //        PaymentOn = first.PaymentOn,
        //        SubjectExpiryDate = first.SubjectExpiryDate,
        //        SubjectMaster = new List<SubjectMasterDto>()
        //    };


        //    // -------------- GROUP SUBJECTS AND BUILD DTOs --------------
        //    var groupedSubjects = data.GroupBy(x => x.Subject.SubjectId);

        //    foreach (var grp in groupedSubjects)
        //    {
        //        var item = grp.First();

        //        // UNITS + CHAPTERS
        //        var unitList = _context.SubjectUnits
        //            .Where(u => u.SubjectId == item.Subject.SubjectId && u.ActiveStatus == 1)
        //            .Select(u => new UnitDto
        //            {
        //                UnitId = u.UnitId.ToString(),
        //                UnitTitle = u.UnitName,
        //                Chapters = _context.SubjectChapters
        //                    .Where(c => c.UnitId == u.UnitId && c.ActiveStatus == 1)
        //                    .OrderBy(c => c.ChapterOrder)
        //                    .Select(c => new ChapterDto
        //                    {
        //                        ChapterId = c.ChapterId.ToString(),
        //                        Title = c.ChapterName
        //                    }).ToList()
        //            }).ToList();

        //        // SUBJECT DTO
        //        var subjectDto = new SubjectMasterDto(_httpContextAccessor, this, _context)
        //        {
        //            SubjectId = item.Subject.SubjectId.ToString(),
        //            SubjectCode = item.Subject.SubjectCode,
        //            SubjectName = item.Subject.SubjectName,
        //            SubjectCoverPath = item.Subject.SubjectCoverPath,
        //            SubjectDescription = item.Subject.SubjectDescription,
        //            Syllabus = item.Subject.Syllabus,
        //            DeptImgPath = item.Subject.DeptImgPath,
        //            Coursehours = item.Subject.Coursehours?.ToString(),
        //            Visuals = item.Subject.Visuals?.ToString(),
        //            Pagecontent = item.Subject.Pagecontent?.ToString(),
        //            Solvedproblem = item.Subject.Solvedproblem?.ToString(),
        //            Multichoice = item.Subject.Multichoice?.ToString(),
        //            DeptVideo = item.Subject.DeptVideo,
        //            IsInDemo = item.Subject.IsInDemo?.ToString(),
        //            TradeId = item.Subject.TradeId?.ToString(),
        //            SubjectSyllabusPath = item.Subject.SubjectSyllabusPath,
        //            Units = unitList
        //        };

        //        package.SubjectMaster.Add(subjectDto);
        //    }

        //    // RETURN ONLY 1 PACKAGE CONTAINING ALL SUBJECTS
        //    return new List<PackageDetailsDTO> { package };
        //}

        public async Task<PackageDetailsDTO> GetPackageDetails(string packageId, int userId)
        {
            long pkgId = Convert.ToInt64(packageId);

            var data = await (
                from pm in _context.TblPackageMasters
                join pd in _context.TblPackageDetails on pm.PackageId equals pd.PackageId
                join sm in _context.TblSubjectMasters on pd.SubjectId equals sm.SubjectId
                join d in _context.TblDepartmentMasters on pd.DepartmentId equals d.DepartmentId
                join usm in _context.TblUserSubscribeMasters
                    on pm.PackageId equals usm.PackageId into usmGroup
                from usm in usmGroup
                    .Where(x => x.UserId == userId && x.PaymentStatus == "Success")
                    .DefaultIfEmpty()
                join sah in _context.TblUserSubjectActivationHistories
                    on usm.UserSubscribeMasterId equals sah.TusmId into sahGroup
                from sah in sahGroup.OrderByDescending(x => x.SubjectExpiryDate).DefaultIfEmpty()
                where pm.PackageId == pkgId && pm.Activestatus == true
                select new
                {
                    pd.DepartmentId,
                    d.DepartmentName,
                    pd.PackageId,
                    pm.PackageName,
                    pm.SellingPrice,
                    pm.CoverPath,
                    pm.PackageDurationDays,
                    Subject = sm,
                    PaymentOn = usm != null ? usm.PaymentOn : null,
                    SubjectExpiryDate = sah != null ? sah.SubjectExpiryDate : null
                }
            ).ToListAsync();

            if (data == null || data.Count == 0)
                return null;

            // VALIDITY
            int validityDays = data.Max(x => x.PackageDurationDays ?? 0);

            // SINGLE PACKAGE OBJECT (your JSON needs this)
            var first = data.First();

            var package = new PackageDetailsDTO
            {
                DepartmentId = first.DepartmentId.ToString(),
                DepartmentName = first.DepartmentName,
                PackageId = first.PackageId.ToString(),
                PackageName = first.PackageName,
                Coverpath = first.CoverPath,
                Price = first.SellingPrice.ToString(),
                Validity = validityDays.ToString(),
                Validitydate = first.SubjectExpiryDate,
                PaymentOn = first.PaymentOn,
                SubjectExpiryDate = first.SubjectExpiryDate,
                IsPurchased = first.PaymentOn != null || first.SubjectExpiryDate != null,
                SubjectMaster = new List<SubjectMasterDto>()
            };

            // GROUP BY SUBJECT
            var groupedSubjects = data.GroupBy(x => x.Subject.SubjectId);

            foreach (var grp in groupedSubjects)
            {
                var item = grp.First();

                // UNITS + CHAPTERS for each subject
                var units = _context.SubjectUnits
                    .Where(u => u.SubjectId == item.Subject.SubjectId && u.ActiveStatus == 1)
                    .Select(u => new UnitDto
                    {
                        UnitId = u.UnitId.ToString(),
                        UnitTitle = u.UnitName,
                        Chapters = _context.SubjectChapters
                            .Where(c => c.UnitId == u.UnitId && c.ActiveStatus == 1)
                            .OrderBy(c => c.ChapterOrder)
                            .Select(c => new ChapterDto
                            {
                                ChapterId = c.ChapterId.ToString(),
                                Title = c.ChapterName
                            }).ToList()
                    }).ToList();

                // SUBJECT DTO
                var subject = new SubjectMasterDto(_httpContextAccessor, this, _context)
                {
                    SubjectId = item.Subject.SubjectId.ToString(),
                    SubjectCode = item.Subject.SubjectCode,
                    SubjectName = item.Subject.SubjectName,
                    SubjectCoverPath = item.Subject.SubjectCoverPath,
                    SubjectDescription = item.Subject.SubjectDescription,
                    Syllabus = item.Subject.Syllabus,
                    DeptImgPath = item.Subject.DeptImgPath,
                    Coursehours = item.Subject.Coursehours?.ToString(),
                    Visuals = item.Subject.Visuals?.ToString(),
                    Pagecontent = item.Subject.Pagecontent?.ToString(),
                    Solvedproblem = item.Subject.Solvedproblem?.ToString(),
                    Multichoice = item.Subject.Multichoice?.ToString(),
                    DeptVideo = item.Subject.DeptVideo,
                    IsInDemo = item.Subject.IsInDemo?.ToString(),
                    TradeId = item.Subject.TradeId?.ToString(),
                    SubjectSyllabusPath = item.Subject.SubjectSyllabusPath,
                    Units = units
                };

                package.SubjectMaster.Add(subject);
            }

            // RETURN EXACT JSON STRUCTURE EXPECTED
            return package;
        }



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
                                //join dsm in _context.TblDepartmentSubjectMappings on pd.DepartmentSubjectMappingId equals dsm.DepartmentSubjectMappingId
                                join sm in _context.TblSubjectMasters on pd.SubjectId equals sm.SubjectId
                                where tpm.PackageId == packageId
                                group new { pd, sm } by tpm into g
                                select new PaymentPackageDTO
                                {
                                    packagemaster = g.Key,
                                    packagedetails = g.Select(x => x.pd).ToList(),
                                    //departmentsubjectmapping = g.Select(x => x.dsm).ToList(),
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
                  packageMasterDto = g
                      .GroupBy(x => x.pm.PackageId)
                      .Select(p => new PackageMasterDto
                      {
                          PackageId = p.First().pm.PackageId,
                          PackageCode = p.First().pm.PackageCode,
                          PackageName = p.First().pm.PackageDisplayName,
                          SellingPrice = p.First().pm.SellingPrice,
                          CoverPath = p.First().pm.CoverPath
                      })
                      .ToList()
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
                                join ah in _context.TblUserSubjectActivationHistories
                                     on usm.UserSubscribeMasterId equals ah.TusmId
                                join pm in _context.TblPackageMasters
                                     on usm.PackageId equals pm.PackageId
                                where usm.PaymentStatus == "success"
                                      && usm.UserId == userId
                                group new { usm, ah, pm } by new
                                {
                                    pm.PackageId,
                                    pm.PackageCode,
                                    pm.PackageDisplayName,
                                    pm.SellingPrice,
                                    pm.CoverPath,
                                    ah.SubjectName
                                } into g
                                orderby g.Max(x => x.usm.PaymentOn) descending
                                select new StudentpurchaseitemsDto
                                {
                                    PackageId = g.Key.PackageId,
                                    PackageCode = g.Key.PackageCode,
                                    packagedisplayname = g.Key.PackageDisplayName,
                                    SellingPrice = g.Key.SellingPrice,
                                    CoverPath = g.Key.CoverPath,
                                    SubjectName = g.Key.SubjectName,
                                    SubjectExpiryDate = g.Max(x => x.ah.SubjectExpiryDate),
                                    PaymentOn = g.Max(x => x.usm.PaymentOn)
                                }
              ).ToListAsync();


            return result;
        }

        public async Task<int> CreatePackageAsync(CreatePackageDto request, int _userid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //get user department
                var user = await _context.TblStudentUserMasters.Where(u => u.StudentUserId == _userid).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                else
                {
                    Console.WriteLine("Department: " + user.DepartmentId);
                }

                // Save Package Master
                var master = new TblPackageMaster
                {
                    //EduType = request.EduType

                    //PackageCode=request.PackageName.Replace(" ","").ToUpper()+DateTime.Now.Ticks.ToString(),
                    PackageCode = "PKG " + request.PackageCode,
                    //PackageDisplayName = request.PackageDisplayName,
                    DepartmentId = user.DepartmentId,
                    PackageName = request.PackageName,
                    PackageDurationDays = request.PackageDurationDays,
                    SellingPrice = request.amount,
                    CoverPath = request.CoverPath,
                    Activestatus = true
                };

                await _context.TblPackageMasters.AddAsync(master);
                await _context.SaveChangesAsync();


                // Save Package Details
                foreach (var sub in request.Subjects)
                {

                    //here i want to map subject id from department subject mapping table
                    var subjectMapping = await _context.TblSubjectMasters.Where(x => x.SubjectCode == sub.Subjectcode).FirstOrDefaultAsync();

                    var detail = new TblPackageDetail
                    {
                        PackageId = master.PackageId,
                        DepartmentId = user.DepartmentId,
                        SubjectId = subjectMapping.SubjectId,

                    };
                    await _context.TblPackageDetails.AddAsync(detail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return master.PackageId;   // return new package id
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //cretae order
        public async Task<CreateOrder> CreateRazorpayOrderRecord(int packageId, string orderid, int createdBy, string status)
        {
            var order = new CreateOrder
            {
                PackageId = packageId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now,
                OrderId = orderid.ToString(),
                Status = status

            };
            _context.CreateOrders.Add(order);
            await _context.SaveChangesAsync();
            return order; // return new order id
        }


        //update order status
        public async Task<CreateOrder> UpdateRazorpayOrderStatus(int orderId, string paymentId, string signature, int userId, string status)
        {
            var order = await _context.CreateOrders.FindAsync(orderId);
            if (order == null)
            {
                return null;
            }
            order.PaymentId = paymentId;
            order.Signature = signature;
            order.Status = status;
            order.UpdatedBy = userId;
            order.UpdatedDate = DateTime.Now;    
            _context.CreateOrders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

    }
}
