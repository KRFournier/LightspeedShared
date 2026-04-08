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
        serviceCollection.AddSingleton<ByeViewModel>();
        serviceCollection.AddSingleton<EmptyParticipantViewModel>();
        serviceCollection.AddTransient<ClockViewModel>();
        serviceCollection.AddTransient<MatchSettingsViewModel>();
        serviceCollection.AddTransient<PlayerViewModel>();
        serviceCollection.AddTransient<PriorityViewModel>();
        serviceCollection.AddTransient<ScoreViewModel>();
        serviceCollection.AddTransient<StandardMatchViewModel>();
    }
}
