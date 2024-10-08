/*
 * EmailSettings.cs
 * This class represents the configuration settings required for sending emails, 
 * including the sender's email, SMTP server information, and the port to use.
 */

public class EmailSettings
{
    // Email address from which the emails will be sent
    public string SenderEmail { get; set; }

    // Client secret for authentication (e.g., for OAuth or API keys)
    public string ClientSecret { get; set; }

    // SMTP server address for sending emails
    public string SmtpServer { get; set; }

    // Port to use for the SMTP server
    public int Port { get; set; }
}
