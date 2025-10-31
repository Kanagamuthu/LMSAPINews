namespace LMSAPI.DTO
{
    public class SyllabusDto
    {
        public int Id { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string UnitTitle { get; set; } = string.Empty;
        public string UnitContent { get; set; } = string.Empty;
        public int PageNumber { get; set; }
    }
}
