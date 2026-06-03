using LRouxTech.Core.Mail;
using Microsoft.Extensions.Options;

namespace LRouxTech.Core.AuthTests.Helpers;

public class EmailSettingsHelper
{
    public static IOptions<EmailSettings> EmailSettingsOptions()
    {
        var emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.mailtrap.io",
            Port = 587,
            Username = "test-smtp-user",
            Password = "test-smtp-password",
            FromAddress = "no-reply@lrouxtech.com",
            FromName = "LRouxTech Auth"
        };

        IOptions<EmailSettings> wrappedSettings = Options.Create(emailSettings);
        
        return wrappedSettings;
    }
}