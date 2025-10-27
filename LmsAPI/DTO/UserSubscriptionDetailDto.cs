using LMSAPI.Models;

namespace LMSAPI.DTO
{
    public class UserSubscriptionDetailDto
    {
        public TblUserSubscribeMaster UserSubscribeMaster { get; set; }
        public TblUserSubjectActivationHistory UserSubjectActivationHistory { get; set; }
    }

}
