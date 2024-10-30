using System.IO;
using AscomPayPG.Models;
using AscomPayPG.Models.DTO;
using Microsoft.Extensions.Configuration;

namespace AscomPayPG.Helpers;

public class EmailComposer
{


    private static readonly IConfiguration _configuration = ConfigurationHelper.GetConfigurationInstance();

    private static string rootdir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    private static string folderPath = "html";

    private static string templateFolder = "templates";
    public static string baseUrl = _configuration["cdn:base_url"];

    public static string ComposeCreditOrDebitNotificationHtmlContent(TransactionAlertNotificationDTO model, bool isCredit)
    {
        //user  amount description time reference currentdate balance
        string body = string.Empty;
        string filename = isCredit ? "Credit_Notification.html" : "Debit_Notification.html";
        string path = Path.Combine(rootdir, folderPath, templateFolder, filename);
        body = File.ReadAllText(path);

        body = body.Replace("{user}", model.User);
        body = body.Replace("{amount}", model.Amount);
        body = body.Replace("{description}", model.Description);
        body = body.Replace("{time}", model.Time);
        body = body.Replace("{reference}", model.Reference);
        body = body.Replace("{currentdate}", model.Currentdate);
        body = body.Replace("{balance}", model.Balance);
        body = isCredit ? body.Replace("{sender}", model.Sender) : body;
        body = !isCredit ? body.Replace("{vat}", model.Vat) : body;
        body = !isCredit ? body.Replace("{charges}", model.Charges) : body;

        return body;
    }
}
