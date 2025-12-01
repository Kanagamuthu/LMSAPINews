namespace LMSAPI.DTO
{
    public class UnitDto
    {
        public string UnitId { get; set; }
        public string UnitTitle { get; set; }
        public List<ChapterDto> Chapters { get; set; } = new();
    }
}
