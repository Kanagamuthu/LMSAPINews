namespace LMSAPI.DTO
{
    public class PackageCreateDto
    {
        public string? degree_id { get; set; }
        public string? department_id { get; set; }
        public string? package_name { get; set; }
        public string? subjects { get; set; }
        public decimal cover_path { get; set; }
        public int amount { get; set; }
        public string? description { get; set; }
    }
}
