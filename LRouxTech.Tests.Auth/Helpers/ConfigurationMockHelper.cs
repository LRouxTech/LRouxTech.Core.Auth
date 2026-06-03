using Microsoft.Extensions.Configuration;
using Moq;

namespace LRouxTech.Tests.Auth.Helpers;

public class ConfigurationMockHelper
{
    public static Mock<IConfiguration> CreateConfigurationMock()
    {
        Mock<IConfiguration> mockConfiguration  =new Mock<IConfiguration>(); 
        
        mockConfiguration
            .Setup(c => c["JwtSettings:SecretKey"])
        .Returns("super-secret-test-key-that-is-long-enough");

        mockConfiguration
            .Setup(c => c["JwtSettings:Issuer"])
        .Returns("https://test-issuer.com");

        mockConfiguration
            .Setup(c => c["JwtSettings:Audience"])
        .Returns("https://test-audience.com");
        
        return mockConfiguration;
    }
}