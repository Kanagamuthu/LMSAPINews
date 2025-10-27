using LMSAPI.DTO;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LMSAPI.Helpers
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
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
