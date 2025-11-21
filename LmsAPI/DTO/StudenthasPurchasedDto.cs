namespace LMSAPI.DTO
{
    public class StudenthasPurchasedDto
    {
        public bool IsPurchased { get; set; }
        public DateTime? SubjectExpiryDate { get; set; }
        public DateTime? _PaymentOn { get; set; }
        public DateTime? _TodayDate { get; set; }
    }
}
