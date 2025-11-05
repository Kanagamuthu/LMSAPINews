using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class ReadHistoryDto
    {
        public int PageCount { get; set; } = 0;
        public int VideoCount { get; set; } = 0;

        public List<TblReadHistory> Downloads { get; set; } = new List<TblReadHistory>();
        public List<TblReadHistory> Bookmarks { get; set; } = new List<TblReadHistory>();
        public List<TblSubjectMaster> Purchase { get; set; } = new List<TblSubjectMaster>();
    }
}
