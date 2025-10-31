namespace LMSAPI.Repository
{
    public interface IStudentDashboard
    {
        Task<object> GetStudentDashboardAsync(int userId);
    }
}
