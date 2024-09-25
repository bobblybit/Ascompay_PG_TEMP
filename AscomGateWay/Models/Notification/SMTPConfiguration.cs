namespace DB_MODALS.Entities;

public class SMTPConfiguration
{
    public string SectionName { get; set; } = "SMTPConfiguration";
    public string EmailFrom { get; set; }
    public int PortNumber { get; set; }
    public string Password { get; set; }
    public bool EnableSSL { get; set; }
    public string UserName { get; set; }
    public string SmtpServer { get; set; }
}
