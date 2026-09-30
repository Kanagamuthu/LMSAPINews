namespace LMSAPI.DTO
{
    /// <summary>
    /// Country dropdown item, sourced from Tbl_CountriesCode.
    /// country_name is the value stored against the student.
    /// </summary>
    public class CountryListDto
    {
        public int id { get; set; }
        public string? name { get; set; }
        //public string? country_code { get; set; }
        //public string? dial_code { get; set; }
    }
}
