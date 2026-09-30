namespace LMSAPI.DTO
{
    public class StudentProfileUpdateDto
    {
        public int educationtype { get; set; }
        public string? studentname { get; set; }
        public string? collegename { get; set; }
        public string? department { get; set; }
        public string? batch { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        // Mandatory. Value comes from the GetCountryList dropdown (country_name).
        public string? country { get; set; }

    }
}
