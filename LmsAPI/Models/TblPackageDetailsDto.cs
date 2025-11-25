namespace LMSAPI.Models
{
    public class TblPackageDetailsDto
    {
        public int PackageDetailId { get; set; }
        public int PackageId { get; set; }
        public int SubjectId { get; set; }

        public TblPackageMaster? Package { get; set; }
    }
}
