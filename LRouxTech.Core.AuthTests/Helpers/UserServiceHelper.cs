using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.AuthTests.TestData.Setup;
using LRouxTech.Core.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace LRouxTech.Core.AuthTests.Helpers;

public class UserServiceHelper
{
    public static IUserService CreateUserService(IUserDbContextFactory factory)
    {
        return new UserService(factory,
            new TokenService(factory, ConfigurationMockHelper.CreateConfigurationMock().Object), new UserValidator(),
            new EmailService(EmailSettingsHelper.EmailSettingsOptions()));
    }
}