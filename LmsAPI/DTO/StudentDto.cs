namespace LMSAPI.DTO
{
    public class StudentDto
    {
        public int StudentUserId { get; set; }

        public string? Name { get; set; }

        public string? EmailId { get; set; }

        public string? MobileNo { get; set; }

        public string? RollNo { get; set; }

        public string? Course { get; set; }

        public bool IsActive { get; set; }
    }
}
