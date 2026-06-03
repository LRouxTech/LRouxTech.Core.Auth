using LRouxTech.Core.Mail;
using Microsoft.Extensions.Options;
using Moq;

namespace LRouxTech.Tests.Auth.Helpers;

public class EmailMockHelper
{
    public static Mock<IEmailService> EmailMock()
    {
        return new Mock<IEmailService>();
    }
}