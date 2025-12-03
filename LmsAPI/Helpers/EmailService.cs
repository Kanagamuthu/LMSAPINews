using LMSAPI.DTO;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LMSAPI.Helpers
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILoggerManager _logger;
        public EmailService(IOptions<SmtpSettings> smtpSettings, ILoggerManager logger)
        {
            _smtpSettings = smtpSettings.Value;
            _logger = logger;
        }
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            
            try
            {
                _logger.LogInfo($"Preparing to send email to {toEmail} with subject '{subject}'.");
                using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
                {
                    //client.EnableSsl = _smtpSettings.EnableSSL;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password);

                    var mailMessage = new MailMessage
                    {
                        //From = new MailAddress(_smtpSettings.UserName, "LMS Support"),
                        From =new MailAddress(_smtpSettings.UserName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);
                    _logger.LogError("Sending email...");
                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
