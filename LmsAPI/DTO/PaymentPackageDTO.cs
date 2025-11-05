using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PaymentPackageDTO
    {
        public TblPackageMaster packagemaster { get; set; }
        public List<TblPackageDetail> packagedetails { get; set; }
        public List<TblDepartmentSubjectMapping> departmentsubjectmapping { get; set; }
        public List<TblSubjectMaster> subjectmaster { get; set; }
    }
}
