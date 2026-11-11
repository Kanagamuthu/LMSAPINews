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
        /// <summary>
        /// Sends an HTML mail. <paramref name="bcc"/> is optional and defaults to none, so
        /// existing callers are unaffected - only the purchase confirmation passes copies.
        /// </summary>
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body,
                                               IEnumerable<string>? bcc = null)
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

                    // A malformed address would throw and lose the whole mail, including the
                    // copy to the student, so bad entries are skipped and logged instead.
                    if (bcc != null)
                    {
                        foreach (var address in bcc)
                        {
                            if (string.IsNullOrWhiteSpace(address))
                                continue;

                            try
                            {
                                mailMessage.Bcc.Add(new MailAddress(address.Trim()));
                            }
                            catch (FormatException)
                            {
                                _logger.LogWarn($"Skipping invalid BCC address '{address}'.");
                            }
                        }

                        if (mailMessage.Bcc.Count > 0)
                            _logger.LogInfo($"BCC: {string.Join(", ", mailMessage.Bcc.Select(a => a.Address))}");
                    }

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
