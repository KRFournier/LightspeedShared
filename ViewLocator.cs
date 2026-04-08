using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Lightspeed.ViewModels;

namespace Lightspeed;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data!.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type is null)
        {
            // look for all lightspeed assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name!.StartsWith("Lightspeed", StringComparison.OrdinalIgnoreCase));

            if (type is null)
            {
                // Search in all loaded assemblies for cross-assembly views
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(name);
                    if (type is not null)
                        break;
                }
            }

            if (type is null)
            {
                foreach (var assembly in assemblies)
                {
                    // Try replacing namespace with assembly name (DLL name)
                    var typeName = data.GetType().Name.Replace("ViewModel", "View");
                    var assemblyName = assembly.GetName().Name;
                    var alternativeName = $"{assemblyName}.Views.{typeName}";
                    type = assembly.GetType(alternativeName);
                    if (type is not null)
                        break;
                }
            }
        }

        if (type is not null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data) => data is ViewModelBase;
}