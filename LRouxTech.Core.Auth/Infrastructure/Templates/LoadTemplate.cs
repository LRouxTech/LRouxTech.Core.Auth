using System.Reflection;

namespace LRouxTech.Core.Auth.Infrastructure.Templates;

public static class LoadTemplate
{
    /// <summary>
    /// Loads an embedded HTML template and replaces dynamic tokens.
    /// </summary>
    public static string RenderTemplate(string templateName, Dictionary<string, string> placeholders)
    {
        var assembly = Assembly.GetExecutingAssembly(); 
        
        string? resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(str => str.EndsWith(templateName, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(resourceName))
        {
            throw new FileNotFoundException($"Could not find embedded template matching '{templateName}'. " +
                                            $"Ensure its Build Action is set to 'Embedded Resource'.");
        }

        using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new StreamReader(stream);
        string htmlContent = reader.ReadToEnd();

        foreach (var placeholder in placeholders)
        {
            htmlContent = htmlContent.Replace($"{{{placeholder.Key}}}", placeholder.Value);
        }

        return htmlContent;
    }
}
