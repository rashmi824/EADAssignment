using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Threading.Tasks;


public class GmailService
{
    private readonly EmailSettings _emailSettings;

    public GmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string recipientEmail, string subject, string message)
{
    var emailMessage = new MimeMessage();
    emailMessage.From.Add(new MailboxAddress("Style Hevan", _emailSettings.SenderEmail));
    emailMessage.To.Add(new MailboxAddress("CSR", recipientEmail));
    emailMessage.Subject = subject;

    var bodyBuilder = new BodyBuilder { TextBody = message };
    emailMessage.Body = bodyBuilder.ToMessageBody();

    using (var smtpClient = new SmtpClient())
    {
        await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.ClientSecret);  // App password used here
        await smtpClient.SendAsync(emailMessage);
        await smtpClient.DisconnectAsync(true);
    }
}

}
