using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LMSAPI.Models;

public partial class LmsdbNewContext : DbContext
{
    public LmsdbNewContext()
    {
    }

    public LmsdbNewContext(DbContextOptions<LmsdbNewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAppConfig> TblAppConfigs { get; set; }

    public virtual DbSet<TblCollegeGroupMap> TblCollegeGroupMaps { get; set; }

    public virtual DbSet<TblCountriesCode> TblCountriesCodes { get; set; }

    public virtual DbSet<TblCountriesMaster> TblCountriesMasters { get; set; }

    public virtual DbSet<TblDegreeMaster> TblDegreeMasters { get; set; }

    public virtual DbSet<TblDepartmentMaster> TblDepartmentMasters { get; set; }

    public virtual DbSet<TblDepartmentSubjectMapping> TblDepartmentSubjectMappings { get; set; }

    public virtual DbSet<TblEducationType> TblEducationTypes { get; set; }

    public virtual DbSet<TblExceptionLog> TblExceptionLogs { get; set; }

    public virtual DbSet<TblOtpverification> TblOtpverifications { get; set; }

    public virtual DbSet<TblPackageDetail> TblPackageDetails { get; set; }

    public virtual DbSet<TblPackageMaster> TblPackageMasters { get; set; }

    public virtual DbSet<TblPaymentMaster> TblPaymentMasters { get; set; }

    public virtual DbSet<TblPersonalInformation> TblPersonalInformations { get; set; }

    public virtual DbSet<TblReadHistory> TblReadHistories { get; set; }

    public virtual DbSet<TblStudentTrialSubject> TblStudentTrialSubjects { get; set; }

    public virtual DbSet<TblStudentUserMaster> TblStudentUserMasters { get; set; }

    public virtual DbSet<TblSubjectMaster> TblSubjectMasters { get; set; }

    public virtual DbSet<TblSubjectMasterHistory> TblSubjectMasterHistories { get; set; }

    public virtual DbSet<TblSubjectUnitMaster> TblSubjectUnitMasters { get; set; }

    public virtual DbSet<TblSubjectUnitMasterHistory> TblSubjectUnitMasterHistories { get; set; }

    public virtual DbSet<TblSupportTicket> TblSupportTickets { get; set; }

    public virtual DbSet<TblUserNotificationDetail> TblUserNotificationDetails { get; set; }

    public virtual DbSet<TblUserNotificationMaster> TblUserNotificationMasters { get; set; }

    public virtual DbSet<TblUserPurchaseGroup> TblUserPurchaseGroups { get; set; }

    public virtual DbSet<TblUserRandomPass> TblUserRandomPasses { get; set; }

    public virtual DbSet<TblUserSubjectActivationHistory> TblUserSubjectActivationHistories { get; set; }

    public virtual DbSet<TblUserSubjectMapping> TblUserSubjectMappings { get; set; }

    public virtual DbSet<TblUserSubjectMappingHistory> TblUserSubjectMappingHistories { get; set; }

    public virtual DbSet<TblUserSubjectReadHistory> TblUserSubjectReadHistories { get; set; }

    public virtual DbSet<TblUserSubscribeMaster> TblUserSubscribeMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAppConfig>(entity =>
        {
            entity.HasKey(e => e.CId).HasName("PK__Tbl_AppC__A9FDEC12D03C3C74");

            entity.ToTable("Tbl_AppConfig");

            entity.Property(e => e.CId).HasColumnName("C_ID");
            entity.Property(e => e.ConfigKey)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("config_key");
            entity.Property(e => e.ConfigValue)
                .HasMaxLength(140)
                .IsUnicode(false)
                .HasColumnName("config_value");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
        });

        modelBuilder.Entity<TblCollegeGroupMap>(entity =>
        {
            entity.HasKey(e => e.CollegeGroupMapId).HasName("PK__Tbl_coll__F1C218E2B421A83A");

            entity.ToTable("Tbl_college_group_map");

            entity.Property(e => e.CollegeGroupMapId).HasColumnName("college_group_map_id");
            entity.Property(e => e.CollegeGroupId).HasColumnName("college_group_id");
            entity.Property(e => e.CollegeId).HasColumnName("college_id");
            entity.Property(e => e.EnterBy).HasColumnName("enter_by");
            entity.Property(e => e.EnterOn)
                .HasColumnType("datetime")
                .HasColumnName("enter_on");
        });

        modelBuilder.Entity<TblCountriesCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Coun__3213E83F174E044D");

            entity.ToTable("Tbl_CountriesCode");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("c_code");
            entity.Property(e => e.CName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("c_name");
            entity.Property(e => e.DialCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("dial_code");
        });

        modelBuilder.Entity<TblCountriesMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_coun__3213E83FDF618DB2");

            entity.ToTable("Tbl_countries_master");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommonName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("common_name");
            entity.Property(e => e.FlagAlt)
                .HasColumnType("text")
                .HasColumnName("flag_alt");
            entity.Property(e => e.FlagPng)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("flag_png");
            entity.Property(e => e.FlagSvg)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("flag_svg");
            entity.Property(e => e.IddRoot)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idd_root");
            entity.Property(e => e.IddSuffixes)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("idd_suffixes");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.OfficialName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("official_name");
        });

        modelBuilder.Entity<TblDegreeMaster>(entity =>
        {
            entity.HasKey(e => e.DId).HasName("PK__Tbl_degr__76B8FF5DD5BB6BA9");

            entity.ToTable("Tbl_degree_master");

            entity.Property(e => e.DId).HasColumnName("D_Id");
            entity.Property(e => e.CrdBy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("crd_by");
            entity.Property(e => e.CrdOn)
                .HasColumnType("datetime")
                .HasColumnName("crd_On");
            entity.Property(e => e.DegreeType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("degree_type");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        modelBuilder.Entity<TblDepartmentMaster>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Tbl_depa__C2232422A6080EF9");

            entity.ToTable("Tbl_department_master");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.Coursehours).HasColumnName("coursehours");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DegreeId).HasColumnName("degree_id");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_code");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("department_name");
            entity.Property(e => e.DeptImgPath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("dept_img_path");
            entity.Property(e => e.DeptVideo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("dept_video");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxSemesterPerYear).HasColumnName("max_semester_per_year");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("modified_on");
            entity.Property(e => e.Multichoice).HasColumnName("multichoice");
            entity.Property(e => e.NoOfYear).HasColumnName("no_of_year");
            entity.Property(e => e.Pagecontent).HasColumnName("pagecontent");
            entity.Property(e => e.Solvedproblem).HasColumnName("solvedproblem");
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");
            entity.Property(e => e.Visuals).HasColumnName("visuals");
        });

        modelBuilder.Entity<TblDepartmentSubjectMapping>(entity =>
        {
            entity.HasKey(e => e.DepartmentSubjectMappingId).HasName("PK__Tbl_Depa__77D5179E59FAB19A");

            entity.ToTable("Tbl_Department_subject_mapping");

            entity.Property(e => e.DepartmentSubjectMappingId).HasColumnName("department_subject_mapping_id");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.MapYear).HasColumnName("map_year");
            entity.Property(e => e.RuleId).HasColumnName("rule_id");
            entity.Property(e => e.Semester).HasColumnName("semester");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.UniversityId).HasColumnName("university_id");
        });

        modelBuilder.Entity<TblEducationType>(entity =>
        {
            entity.HasKey(e => e.EduId).HasName("PK__Tbl_Educ__E5726E42EE7AE71B");

            entity.ToTable("Tbl_EducationType");

            entity.Property(e => e.EduId).HasColumnName("Edu_ID");
            entity.Property(e => e.EduCode)
                .HasMaxLength(5)
                .HasColumnName("Edu_Code");
            entity.Property(e => e.EduDes)
                .HasMaxLength(80)
                .HasColumnName("Edu_Des");
        });

        modelBuilder.Entity<TblExceptionLog>(entity =>
        {
            entity.HasKey(e => e.ExceptionId).HasName("PK__Tbl_Exce__26981DA826ED7DFF");

            entity.ToTable("Tbl_ExceptionLog");

            entity.Property(e => e.ExceptionId).HasColumnName("ExceptionID");
            entity.Property(e => e.ErrorLine).HasColumnName("errorLine");
            entity.Property(e => e.ExceptionDate).HasColumnType("datetime");
            entity.Property(e => e.ExceptionMessage).IsUnicode(false);
            entity.Property(e => e.ExceptionSource)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExceptionType)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ExceptionUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ExceptionURL");
            entity.Property(e => e.HelpLink)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsFromController).HasColumnName("isFromController");
            entity.Property(e => e.StackTrace).IsUnicode(false);
            entity.Property(e => e.TargetSiteModule)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TargetSiteName)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblOtpverification>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("PK__Tbl_OTPV__40A54F5385EA87E5");

            entity.ToTable("Tbl_OTPVerification");

            entity.Property(e => e.OtpId).HasColumnName("OTP_ID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.ExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("OTP");
            entity.Property(e => e.UId).HasColumnName("U_ID");
        });

        modelBuilder.Entity<TblPackageDetail>(entity =>
        {
            entity.HasKey(e => e.PackageDetailId).HasName("PK__Tbl_pack__CB759BDC26B7DE10");

            entity.ToTable("Tbl_package_details");

            entity.Property(e => e.PackageDetailId).HasColumnName("package_detail_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentSubjectMappingId).HasColumnName("department_subject_mapping_id");
            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.Semester).HasColumnName("semester");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectUnitType).HasColumnName("subject_unit_type");
            entity.Property(e => e.ValidityDays).HasColumnName("validity_days");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<TblPackageMaster>(entity =>
        {
            entity.HasKey(e => e.PackageId).HasName("PK__Tbl_pack__63846AE845D38897");

            entity.ToTable("Tbl_package_master");

            entity.Property(e => e.PackageId).HasColumnName("package_id");
            entity.Property(e => e.Activestatus).HasColumnName("activestatus");
            entity.Property(e => e.ActualPrice).HasColumnName("actual_price");
            entity.Property(e => e.CoverPath)
                .IsUnicode(false)
                .HasColumnName("cover_path");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.CurrentStatus).HasColumnName("current_status");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.IsBundle).HasColumnName("is_bundle");
            entity.Property(e => e.IsOfferPackage).HasColumnName("is_offer_package");
            entity.Property(e => e.Keywords)
                .IsUnicode(false)
                .HasColumnName("keywords");
            entity.Property(e => e.LongDesc)
                .IsUnicode(false)
                .HasColumnName("long_desc");
            entity.Property(e => e.OsType).HasColumnName("os_type");
            entity.Property(e => e.PackageCode)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("package_code");
            entity.Property(e => e.PackageDisplayName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("package_display_name");
            entity.Property(e => e.PackageDurationDays).HasColumnName("package_duration_days");
            entity.Property(e => e.PackageName)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("package_name");
            entity.Property(e => e.PackageVideoUrl)
                .IsUnicode(false)
                .HasColumnName("package_video_url");
            entity.Property(e => e.RuleId).HasColumnName("rule_id");
            entity.Property(e => e.SellingPrice).HasColumnName("selling_price");
            entity.Property(e => e.Semester).HasColumnName("semester");
            entity.Property(e => e.ShortDesc)
                .IsUnicode(false)
                .HasColumnName("short_desc");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectUnitType).HasColumnName("subject_unit_type");
            entity.Property(e => e.UnivId).HasColumnName("univ_id");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<TblPaymentMaster>(entity =>
        {
            entity.HasKey(e => e.PaymentMasterId).HasName("PK__Tbl_paym__14DD623F9521FB37");

            entity.ToTable("Tbl_payment_master");

            entity.Property(e => e.PaymentMasterId).HasColumnName("payment_master_id");
            entity.Property(e => e.BalAmt)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("bal_amt");
            entity.Property(e => e.BaseFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("base_fee");
            entity.Property(e => e.ClintTxnRefNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PaymentGatwayId).HasColumnName("payment_gatway_id");
            entity.Property(e => e.RefundAmt)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("refund_amt");
            entity.Property(e => e.RefundDate)
                .HasColumnType("datetime")
                .HasColumnName("refund_date");
            entity.Property(e => e.RequestToken)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("request_token");
            entity.Property(e => e.RpstToken)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rpst_token");
            entity.Property(e => e.ServiceFee)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("service_fee");
            entity.Property(e => e.SmsStatus).HasColumnName("sms_status");
            entity.Property(e => e.TpslCharges)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Tpsl_charges");
            entity.Property(e => e.TpslrefundId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TPSLRefundID");
            entity.Property(e => e.TpsltxnBankCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TPSLTxnBankCode");
            entity.Property(e => e.TpsltxnId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TPSLTxnID");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TxnAmount).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TxnDate).HasColumnType("datetime");
            entity.Property(e => e.TxnDateTime).HasColumnType("datetime");
            entity.Property(e => e.TxnErrMsg)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("Txn_err_msg");
            entity.Property(e => e.TxnMsg)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Txn_msg");
            entity.Property(e => e.TxnStatus)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserSubscribeMasterId).HasColumnName("user_subscribe_master_id");
        });

        modelBuilder.Entity<TblPersonalInformation>(entity =>
        {
            entity.HasKey(e => e.UId).HasName("PK__Tbl_Pers__5A2040BBFC366AEE");

            entity.ToTable("Tbl_PersonalInformation");

            entity.Property(e => e.UId).HasColumnName("U_Id");
            entity.Property(e => e.College).HasMaxLength(255);
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(255);
            entity.Property(e => e.DeviceMacId).HasMaxLength(100);
            entity.Property(e => e.Education).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Platform).HasMaxLength(50);
            entity.Property(e => e.ProfileType).HasMaxLength(50);
            entity.Property(e => e.Trades).HasMaxLength(350);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserStatus).HasMaxLength(100);
        });

        modelBuilder.Entity<TblReadHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Read__3214EC07191B876E");

            entity.ToTable("Tbl_ReadHistory");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblStudentTrialSubject>(entity =>
        {
            entity.HasKey(e => e.UserTrialSubjectId).HasName("PK__Tbl_stud__D9318AA9A70D4EA3");

            entity.ToTable("Tbl_student_trial_subject");

            entity.Property(e => e.UserTrialSubjectId).HasColumnName("user_trial_subject_id");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TradeActiveStatus).HasColumnName("trade_active_status");
            entity.Property(e => e.TrailExpiryOn)
                .HasColumnType("datetime")
                .HasColumnName("trail_expiry_on");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TblStudentUserMaster>(entity =>
        {
            entity.HasKey(e => e.StudentUserId).HasName("PK__Tbl_Stud__6E2C68F7819E5F39");

            entity.ToTable("Tbl_Student_user_master");

            entity.Property(e => e.StudentUserId).HasColumnName("student_user_id");
            entity.Property(e => e.AccActiveOn)
                .HasColumnType("datetime")
                .HasColumnName("acc_active_on");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.Batchyear)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("batchyear");
            entity.Property(e => e.Collegename)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("collegename");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.CountryCode).HasMaxLength(5);
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_name");
            entity.Property(e => e.EduType).HasColumnName("edu_type");
            entity.Property(e => e.EmailId)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("email_id");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("mobile");
            entity.Property(e => e.PrimaryImei)
                .HasMaxLength(50)
                .HasColumnName("primary_imei");
            entity.Property(e => e.PrimaryMac)
                .HasMaxLength(60)
                .HasColumnName("primary_MAC");
            entity.Property(e => e.TradeId).HasColumnName("trade_id");
            entity.Property(e => e.UserFirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_first_name");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<TblSubjectMaster>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Tbl_subj__5004F6608A1CC90F");

            entity.ToTable("Tbl_subject_master");

            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.ActiveDurationDate)
                .HasColumnType("datetime")
                .HasColumnName("active_duration_date");
            entity.Property(e => e.ActiveDurationDays).HasColumnName("active_duration_days");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.Coursehours).HasColumnName("coursehours");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DeptImgPath)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("dept_img_path");
            entity.Property(e => e.DeptVideo)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("dept_video");
            entity.Property(e => e.HavingQuestionpaper).HasColumnName("having_questionpaper");
            entity.Property(e => e.IsInDemo).HasColumnName("is_inDemo");
            entity.Property(e => e.IsInTrail).HasColumnName("is_inTrail");
            entity.Property(e => e.Multichoice).HasColumnName("multichoice");
            entity.Property(e => e.Pagecontent).HasColumnName("pagecontent");
            entity.Property(e => e.ReleasedOn)
                .HasColumnType("datetime")
                .HasColumnName("released_on");
            entity.Property(e => e.RuleId).HasColumnName("rule_id");
            entity.Property(e => e.Solvedproblem).HasColumnName("solvedproblem");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(100)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectCoverPath).HasColumnName("subject_cover_path");
            entity.Property(e => e.SubjectDescription).HasColumnName("subject_description");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectSyllabusPath).HasColumnName("Subject_Syllabus_Path");
            entity.Property(e => e.SubjectVersion)
                .HasMaxLength(20)
                .HasColumnName("subject_version");
            entity.Property(e => e.Syllabus).HasColumnName("syllabus");
            entity.Property(e => e.TradeId).HasColumnName("trade_id");
            entity.Property(e => e.UnivSubjectCode)
                .HasMaxLength(30)
                .HasColumnName("univ_subject_code");
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");
            entity.Property(e => e.Visuals).HasColumnName("visuals");
        });

        modelBuilder.Entity<TblSubjectMasterHistory>(entity =>
        {
            entity.HasKey(e => e.SubjectHistoryid).HasName("PK__Tbl_subj__152EEBC31BA8ACC4");

            entity.ToTable("Tbl_subject_master_history");

            entity.Property(e => e.SubjectHistoryid).HasColumnName("subject_historyid");
            entity.Property(e => e.ActiveDurationDate)
                .HasColumnType("datetime")
                .HasColumnName("active_duration_date");
            entity.Property(e => e.ActiveDurationDays).HasColumnName("active_duration_days");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.HavingQuestionpaper).HasColumnName("having_questionpaper");
            entity.Property(e => e.ReleasedOn)
                .HasColumnType("datetime")
                .HasColumnName("released_on");
            entity.Property(e => e.RuleId).HasColumnName("rule_id");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(20)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectCoverPath).HasColumnName("subject_cover_path");
            entity.Property(e => e.SubjectDescription).HasColumnName("subject_description");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(100)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectVersion)
                .HasMaxLength(15)
                .HasColumnName("subject_version");
            entity.Property(e => e.UniversityId).HasColumnName("UniversityID");
        });

        modelBuilder.Entity<TblSubjectUnitMaster>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Tbl_subj__D3AF5BD7392AFDDE");

            entity.ToTable("Tbl_subject_unit_master");

            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.FilesizeInMb).HasColumnName("filesize_in_mb");
            entity.Property(e => e.IsDemo).HasColumnName("is_demo");
            entity.Property(e => e.IsUnitOrIndex).HasColumnName("is_unit_or_index");
            entity.Property(e => e.OsType).HasColumnName("os_type");
            entity.Property(e => e.ReleasedOn)
                .HasColumnType("datetime")
                .HasColumnName("released_on");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectUnitPath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("subject_unit_path");
            entity.Property(e => e.SubjectUnitType).HasColumnName("subject_unit_type");
            entity.Property(e => e.SubjectUnitVersion)
                .HasMaxLength(20)
                .HasColumnName("subject_unit_version");
            entity.Property(e => e.UnitCode)
                .HasMaxLength(50)
                .HasColumnName("unit_code");
            entity.Property(e => e.UnitName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<TblSubjectUnitMasterHistory>(entity =>
        {
            entity.HasKey(e => e.UnitHistoryId).HasName("PK__Tbl_subj__64C77099F745C805");

            entity.ToTable("Tbl_subject_unit_master_history");

            entity.Property(e => e.UnitHistoryId).HasColumnName("unit_history_id");
            entity.Property(e => e.ActiveStatus).HasColumnName("active_status");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.FilesizeInMb).HasColumnName("filesize_in_mb");
            entity.Property(e => e.IsDemo).HasColumnName("is_demo");
            entity.Property(e => e.IsUnitOrIndex).HasColumnName("is_unit_or_index");
            entity.Property(e => e.ReleasedOn)
                .HasColumnType("datetime")
                .HasColumnName("released_on");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectUnitPath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("subject_unit_path");
            entity.Property(e => e.SubjectUnitVersion)
                .HasMaxLength(20)
                .HasColumnName("subject_unit_version");
            entity.Property(e => e.UnitCode)
                .HasMaxLength(25)
                .HasColumnName("unit_code");
            entity.Property(e => e.UnitId).HasColumnName("unit_id");
            entity.Property(e => e.UnitName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<TblSupportTicket>(entity =>
        {
            entity.HasKey(e => e.StId).HasName("PK__Tbl_Supp__A85E81CFAC6D192C");

            entity.ToTable("Tbl_SupportTicket");

            entity.Property(e => e.StId).HasColumnName("st_id");
            entity.Property(e => e.ActiveStatus).HasColumnName("Active_status");
            entity.Property(e => e.Createdon)
                .HasColumnType("datetime")
                .HasColumnName("createdon");
            entity.Property(e => e.EmailId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email_id");
            entity.Property(e => e.Isread).HasColumnName("isread");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.ReadBy).HasColumnName("read_by");
            entity.Property(e => e.Readon)
                .HasColumnType("datetime")
                .HasColumnName("readon");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("subject");
        });

        modelBuilder.Entity<TblUserNotificationDetail>(entity =>
        {
            entity.HasKey(e => e.UserNotifyId).HasName("PK__Tbl_user__7F53E0C02C24D61D");

            entity.ToTable("Tbl_user_notification_details");

            entity.Property(e => e.UserNotifyId).HasColumnName("user_notify_id");
            entity.Property(e => e.Isread).HasColumnName("isread");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.NotificationOn)
                .HasColumnType("datetime")
                .HasColumnName("notification_on");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TblUserNotificationMaster>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Tbl_user__E059842FF3A20951");

            entity.ToTable("Tbl_user_notification_master");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.Isread).HasColumnName("isread");
            entity.Property(e => e.NotificationFor).HasColumnName("notification_for");
            entity.Property(e => e.NotificationMessage).HasColumnName("notification_message");
        });

        modelBuilder.Entity<TblUserPurchaseGroup>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Tbl_user__149AF30AF0D93C43");

            entity.ToTable("Tbl_user_purchase_group");

            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.IsActive).HasColumnName("Is_Active");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubjectCode).HasColumnName("subject_code");
        });

        modelBuilder.Entity<TblUserRandomPass>(entity =>
        {
            entity.HasKey(e => e.UserRandomId).HasName("PK__Tbl_user__F3F6B8D7A4013A75");

            entity.ToTable("Tbl_user_random_pass");

            entity.Property(e => e.UserRandomId).HasColumnName("user_random_id");
            entity.Property(e => e.ActionType).HasColumnName("action_type");
            entity.Property(e => e.GeneratedTime)
                .HasColumnType("datetime")
                .HasColumnName("generated_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserType).HasColumnName("user_type");
            entity.Property(e => e.VerificationCode)
                .IsUnicode(false)
                .HasColumnName("verification_code");
        });

        modelBuilder.Entity<TblUserSubjectActivationHistory>(entity =>
        {
            entity.HasKey(e => e.UserSubjectActivationId).HasName("PK__Tbl_user__BDA58D701ED814D3");

            entity.ToTable("Tbl_user_subject_activation_history");

            entity.Property(e => e.UserSubjectActivationId).HasColumnName("user_subject_activation_id");
            entity.Property(e => e.ActivatedBy).HasColumnName("activated_by");
            entity.Property(e => e.ActivatedOn)
                .HasColumnType("datetime")
                .HasColumnName("activated_on");
            entity.Property(e => e.ActivationProductType).HasColumnName("activation_product_type");
            entity.Property(e => e.ActivationType).HasColumnName("activation_type");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(30)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("subject_expiry_date");
            entity.Property(e => e.SubjectExpiryExtensionDate)
                .HasColumnType("datetime")
                .HasColumnName("subject_expiry_extension_date");
            entity.Property(e => e.SubjectExtensionDays).HasColumnName("subject_extension_days");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_version");
            entity.Property(e => e.TusmId).HasColumnName("TUSM_Id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TblUserSubjectMapping>(entity =>
        {
            entity.HasKey(e => e.UserSubjectId).HasName("PK__tbl_user__56310F0CC55BA4AB");

            entity.ToTable("tbl_user_subject_mapping");

            entity.Property(e => e.UserSubjectId).HasColumnName("user_subject_id");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_code");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_name");
            entity.Property(e => e.DownloadStatus).HasColumnName("download_status");
            entity.Property(e => e.DownloadedOn)
                .HasColumnType("datetime")
                .HasColumnName("downloaded_on");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDemo).HasColumnName("is_demo");
            entity.Property(e => e.IsPurchased).HasColumnName("is_purchased");
            entity.Property(e => e.IsTrial).HasColumnName("is_trial");
            entity.Property(e => e.IsUnitOrIndex).HasColumnName("is_unit_or_index");
            entity.Property(e => e.IsUpdatedSubunitToClient).HasColumnName("is_updated_subunit_to_client");
            entity.Property(e => e.IsUpdatedToClient).HasColumnName("is_updated_to_client");
            entity.Property(e => e.LastClientUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("last_client_updated_on");
            entity.Property(e => e.LastModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("last_modified_on");
            entity.Property(e => e.LastSubunitClientUpdatedOn)
                .HasColumnType("datetime")
                .HasColumnName("last_subunit_client_updated_on");
            entity.Property(e => e.LastSubunitModifiedOn)
                .HasColumnType("datetime")
                .HasColumnName("last_subunit_modified_on");
            entity.Property(e => e.PackageExpirydate)
                .HasColumnType("datetime")
                .HasColumnName("package_expirydate");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(30)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectCoverPath).HasColumnName("subject_cover_path");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectPurchasedon)
                .HasColumnType("datetime")
                .HasColumnName("subject_purchasedon");
            entity.Property(e => e.SubjectTrialExpiryon)
                .HasColumnType("datetime")
                .HasColumnName("subject_trial_expiryon");
            entity.Property(e => e.SubjectUnitCode)
                .HasMaxLength(50)
                .HasColumnName("subject_unit_code");
            entity.Property(e => e.SubjectUnitExpiryon)
                .HasColumnType("datetime")
                .HasColumnName("subject_unit_expiryon");
            entity.Property(e => e.SubjectUnitId).HasColumnName("subject_unit_id");
            entity.Property(e => e.SubjectUnitName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("subject_unit_name");
            entity.Property(e => e.SubjectUnitPath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("subject_unit_path");
            entity.Property(e => e.SubjectUnitUserVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_unit_user_version");
            entity.Property(e => e.SubjectUnitVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_unit_version");
            entity.Property(e => e.SubjectVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_version");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Yearsem).HasColumnName("yearsem");
        });

        modelBuilder.Entity<TblUserSubjectMappingHistory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Tbl_user_subject_mapping_history");

            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_code");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("department_name");
            entity.Property(e => e.DownloadStatus).HasColumnName("download_status");
            entity.Property(e => e.DownloadedOn)
                .HasColumnType("datetime")
                .HasColumnName("downloaded_on");
            entity.Property(e => e.IsDemo).HasColumnName("is_demo");
            entity.Property(e => e.IsUnitOrIndex).HasColumnName("is_unit_or_index");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(20)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectCoverPath).HasColumnName("subject_cover_path");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("subject_name");
            entity.Property(e => e.SubjectUnitCode)
                .HasMaxLength(25)
                .HasColumnName("subject_unit_code");
            entity.Property(e => e.SubjectUnitExpiryon)
                .HasColumnType("datetime")
                .HasColumnName("subject_unit_expiryon");
            entity.Property(e => e.SubjectUnitId).HasColumnName("subject_unit_id");
            entity.Property(e => e.SubjectUnitName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("subject_unit_name");
            entity.Property(e => e.SubjectUnitPath)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("subject_unit_path");
            entity.Property(e => e.SubjectUnitVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_unit_version");
            entity.Property(e => e.SubjectVersion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("subject_version");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSubjectHistoryId)
                .ValueGeneratedOnAdd()
                .HasColumnName("user_subject_history_id");
            entity.Property(e => e.UserSubjectId).HasColumnName("user_subject_id");
        });

        modelBuilder.Entity<TblUserSubjectReadHistory>(entity =>
        {
            entity.HasKey(e => e.UserReadHistoryId).HasName("PK__Tbl_user__FE2597B1A2A00CF3");

            entity.ToTable("Tbl_user_subject_read_history");

            entity.Property(e => e.UserReadHistoryId).HasColumnName("user_read_history_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.EnteredOn)
                .HasColumnType("datetime")
                .HasColumnName("entered_on");
            entity.Property(e => e.HoursTopicVideo)
                .HasColumnType("text")
                .HasColumnName("hours_topic_video");
            entity.Property(e => e.IsUpdatedToServer).HasColumnName("is_updated_to_server");
            entity.Property(e => e.LastReadOn)
                .HasColumnType("datetime")
                .HasColumnName("last_read_on");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.TotalHours).HasColumnName("total_hours");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<TblUserSubscribeMaster>(entity =>
        {
            entity.HasKey(e => e.UserSubscribeMasterId).HasName("PK__Tbl_user__E5DF7BB1526BFB16");

            entity.ToTable("Tbl_user_subscribe_master");

            entity.Property(e => e.UserSubscribeMasterId).HasColumnName("user_subscribe_master_id");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.BillingAddressId).HasColumnName("billing_address_id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.CreatedOn)
                .HasColumnType("datetime")
                .HasColumnName("created_on");
            entity.Property(e => e.DeliverAddressId).HasColumnName("deliver_address_id");
            entity.Property(e => e.DiscountAmt).HasColumnName("discount_amt");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.OrderStatus).HasColumnName("order_status");
            entity.Property(e => e.PaymentOn)
                .HasColumnType("datetime")
                .HasColumnName("payment_on");
            entity.Property(e => e.PaymentRefNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_ref_no");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(30)
                .HasColumnName("payment_status");
            entity.Property(e => e.RefundAmount).HasColumnName("refund_amount");
            entity.Property(e => e.RefundBy).HasColumnName("refund_by");
            entity.Property(e => e.RefundOn)
                .HasColumnType("datetime")
                .HasColumnName("refund_on");
            entity.Property(e => e.TransactionType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserSubscribeDeliveryModeId).HasColumnName("user_subscribe_delivery_mode_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
