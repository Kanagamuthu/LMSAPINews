using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class ReadHistoryDto
    {
        public int PageCount { get; set; } = 0;
        public int VideoCount { get; set; } = 0;

        public List<TblReadHistory> tblreadhistory { get; set; } = new List<TblReadHistory>();
    }
}
