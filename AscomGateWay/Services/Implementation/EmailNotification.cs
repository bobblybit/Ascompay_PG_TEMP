using System.Net.Mail;
using System.Net;
using MimeKit;
using AscomPayPG.Helpers;
using AscomPayPG.Services.Interface;
using AscomPayPG.Models.Notification;

namespace AscomPayPG.Services.Implementation;

public class EmailNotification : IEmailNotification
{
    private readonly IConfiguration _configuration = ConfigurationHelper.GetConfigurationInstance();
    private readonly SMTPConfiguration _smtpConfiguration = new SMTPConfiguration();
    public EmailNotification()
    {
        _configuration.Bind(_smtpConfiguration.SectionName, _smtpConfiguration);
    }
    public Task<bool> AddNewEmailConfiguration(SMTPConfiguration sMTPConfiguration)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> NewSendEmail(string DisplayName, string Subject, string HTMLMessage, string Receiver)
    {
        bool sendMail = false;
        string serverName = _smtpConfiguration.SmtpServer;
        int portNumber = _smtpConfiguration.PortNumber;
        bool ssl = _smtpConfiguration.EnableSSL;
        string fromMail = _smtpConfiguration.EmailFrom;
        string userName = _smtpConfiguration.UserName;
        string password = _smtpConfiguration.Password;
        MailMessage mail = new MailMessage();                 //Setting From , To and CC

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Ascom Pay", fromMail));
        message.To.Add(new MailboxAddress(DisplayName, Receiver));
        message.Subject = Subject;
        message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = HTMLMessage
        };


        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            client.Connect(serverName, portNumber, ssl);
            client.Authenticate(userName, password);
            client.Send(message);
            client.Disconnect(true);
            sendMail = true;
        }
        return sendMail;
    }

    public async Task<bool> SendEmail(string DisplayName, string Subject, string HTMLMessage, List<string> Receivers)
    {
        bool mailSent = false;
        SmtpClient client = new SmtpClient(_smtpConfiguration.SmtpServer, _smtpConfiguration.PortNumber);
        client.EnableSsl = _smtpConfiguration.EnableSSL; // also iis has a default session time out, but i don't think that should be causing this
        client.Timeout = 200000;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(_smtpConfiguration.UserName, _smtpConfiguration.Password);
        MailMessage mail = new MailMessage();                 //Setting From , To and CC
        mail.From = new MailAddress(_smtpConfiguration.EmailFrom, DisplayName);

        //adding recieptients
        foreach (var item in Receivers)
        {
            mail.To.Add(new MailAddress(item));
        }

        // mail.CC.Add(new MailAddress("xyz")); 
        mail.Subject = Subject;
        mail.Body = HTMLMessage;
        mail.IsBodyHtml = true;
        mail.Priority = MailPriority.High;

        try
        {
            client.Send(mail);
            mailSent = true;
        }
        catch (Exception ex)
        {
            mailSent = false;
        }
        return mailSent;
    }
}