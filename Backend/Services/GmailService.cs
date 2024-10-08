// GmailService.cs
// This class is responsible for sending emails using Gmail's SMTP server.
// It uses the MailKit library to create and send emails asynchronously.

using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Threading.Tasks;

public class GmailService
{
    private readonly EmailSettings _emailSettings; // Holds email configuration settings

    public GmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value; // Initialize email settings from configuration
    }

    // Sends an email asynchronously to the specified recipient.
    public async Task SendEmailAsync(string recipientEmail, string subject, string message)
    {
        // Create a new email message
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Style Hevan", _emailSettings.SenderEmail)); // Sender's name and email
        emailMessage.To.Add(new MailboxAddress("CSR", recipientEmail)); // Recipient's name and email
        emailMessage.Subject = subject; // Email subject

        // Build the email body
        var bodyBuilder = new BodyBuilder { TextBody = message }; // Plain text body
        emailMessage.Body = bodyBuilder.ToMessageBody(); // Set the email body

        // Use SmtpClient to send the email
        using (var smtpClient = new SmtpClient())
        {
            // Connect to the SMTP server with TLS encryption
            await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
            
            // Authenticate with the sender's email and application password
            await smtpClient.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.ClientSecret);
            
            // Send the email message
            await smtpClient.SendAsync(emailMessage);
            
            // Disconnect from the SMTP server
            await smtpClient.DisconnectAsync(true);
        }
    }
}
