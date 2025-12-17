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
   

    public class readhistorydto
    {
        public string departmentName { get; set; }
        public List<Packagemasterdto> packageMasterDto { get; set; }
    }

    public class Packagemasterdto
    {
        public int packageId { get; set; }
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public string sellingPrice { get; set; }
        public string coverPath { get; set; }
        public bool isPurchased { get; set; }
        public object subjectExpiryDate { get; set; }
        public object paymentOn { get; set; }
        public string TransactionType { get; set; }
        public string? discount { get; set; }
        public string? dealname { get; set; }
    }

}
