namespace LMSAPI.DTO
{
    public class ChapterDto
    {
        public string ChapterId { get; set; }    // NEW ID
        public string? Title { get; set; }
        //public string? Url { get; set; }       // new url format

        public bool? isread { get; set; }
    }
}
