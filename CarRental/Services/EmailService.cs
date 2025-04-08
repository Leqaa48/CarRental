using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CarRental.Models;
using Microsoft.Extensions.Options;

public class EmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", _smtpSettings.Username));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using (var client = new SmtpClient())
        {
            client.Connect(_smtpSettings.Host, _smtpSettings.Port, _smtpSettings.EnableSsl);
            client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
            client.Send(message);
            client.Disconnect(true);
        }
    
}
}
