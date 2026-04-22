using Lightspeed.Services;
using Lightspeed.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LightspeedShared;

public static class SharedDependencies
{
    /// <summary>
    /// Registers all shared viewmodels and services
    /// </summary>
    public static void Register(ServiceCollection serviceCollection)
    {
        // services
        serviceCollection.AddTransient<MatchFactory>();
        serviceCollection.AddTransient<SharedLoadingService>();

        // view models
        serviceCollection.AddTransient<BracketMatchViewModel>();
        serviceCollection.AddTransient<StandardMatchSettingsViewModel>();
        serviceCollection.AddTransient<StandardMatchViewModel>();

        // note: we do not register match components. Those are instantiated by
        // the matches as needed, and are not shared across matches.
    }
}
