namespace LMSAPI.DTO
{
    public class StudentTradeDepartmentDTO
    {
        public int? edutype { get; set; }
        //public int? DepartmentId { get; set; }
        public string? department_name { get; set; }
        public string? batchyear { get; set; }
        public string? collegename { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        // Mandatory. Value comes from the GetCountryList dropdown (country_name).
        public string? country { get; set; }
        //public string? SubjectName { get; set; }
        //public int? TradeId { get; set; }
    }
}
