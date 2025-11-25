namespace LMSAPI.DTO
{
    public class PackageResponseDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Duration { get; set; } = string.Empty;
        public List<PackageDetailsDTO> PackageDetails { get; set; } = new List<PackageDetailsDTO>();
    }
}
