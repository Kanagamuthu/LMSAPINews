using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class PaymentPayload
    {
        public string subjectCode { get; set; }
        public int DepartmentId { get; set; }
        public TblUserSubscribeMaster SubscribeMaster { get; set; }
    }
}
