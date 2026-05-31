using System.Reflection;

namespace LRouxTech.Core.Auth.Infrastructure.Templates;

public static class LoadTemplate
{
    public static string LoadEmbeddedTemplate(string templateName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourcePath = $"LRouxTech.Core.Mail.Templates.{templateName}.html";

        using Stream stream = assembly.GetManifestResourceStream(resourcePath) 
                              ?? throw new FileNotFoundException($"Could not find embedded email template: {resourcePath}");
            
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
