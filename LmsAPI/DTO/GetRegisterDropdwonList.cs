using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class GetRegisterDropdwonList
    {
        public List<TblDepartmentMaster>? DepartmentList { get; set; }
        public List<TblDegreeMaster>? DegreeList { get; set; }
    }
}
