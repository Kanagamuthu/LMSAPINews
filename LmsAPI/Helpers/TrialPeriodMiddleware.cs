using LMSAPI.Helpers;
using LMSAPI.Repository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LMSAPI.Helpers
{
    public class TrialPeriodMiddleware
    {
        private readonly RequestDelegate _next;

        public TrialPeriodMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IStudentsRepository studentsRepository, IDashboardRepository dashboardRepository)
        {
            var ServerTime = DateTime.Now.ToString("yyyy-MM-dd");
            try
            {
                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    var allowedEndpoints = new[] { "GetAllPackage", "GetPackageDetails", "TrailSubscription", "CreateSubscription" };

                    if (endpoint?.DisplayName != null && allowedEndpoints.Any(name => endpoint.DisplayName.Contains(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var user = context.User;
                        if (user?.Identity?.IsAuthenticated == true)
                        {
                            var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                            if (!string.IsNullOrEmpty(email) && long.TryParse(userIdClaim, out long userId))
                            {
                                var student = await studentsRepository.GetStudentByEmailAsync(email);
                                if (student != null && student.AccActiveOn.HasValue)
                                {
                                    var trialDays = await studentsRepository.GetTrialPeriodDaysAsync();
                                    var activationDate = student.AccActiveOn.Value;
                                    var daysSinceActivation = (DateTime.Now - activationDate).Days;
                                    int daysLeft = trialDays - daysSinceActivation;
                                    if (daysLeft <= 0)
                                    {
                                        // trial expired
                                        await dashboardRepository.SetInactive(userId);

                                        context.Items["TrialInfo"] = new { InTrialPeriod = false, DaysLeft = 0 , ServerTime = ServerTime };
                                    }
                                    else
                                    {
                                        context.Items["TrialInfo"] = new { InTrialPeriod = true, DaysLeft = daysLeft, ServerTime = ServerTime };
                                    }
                                }
                                else
                                {
                                    context.Items["TrialInfo"] = new { InTrialPeriod = false, DaysLeft = 0 , ServerTime = ServerTime };
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                context.Items["TrialInfo"] = new { InTrialPeriod = false, DaysLeft = 0, ServerTime= ServerTime };
            }

            await _next(context);
        }
    }

}
