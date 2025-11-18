using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class DepartmentMasterDto
    {
        public int? Id { get; set; }
        public string? Department_name { get; set; }
        public string? Department_description { get;set; }
        public int? Edu_type_id { get; set; }

    }
}
