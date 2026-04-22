using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.MatchComponents;
using Lightspeed.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.ViewModels;

/// <summary>
/// A match
/// </summary>
public partial class StandardMatchViewModel : MatchViewModel
{
    #region Properties

    [ObservableProperty]
    public partial StandardMatchSettingsViewModel Settings { get; set; }

    [ObservableProperty]
    public partial ClockViewModel Clock { get; set; }

    [ObservableProperty]
    public partial LeftRightViewModel Scores { get; set; }

    [ObservableProperty]
    public partial PriorityViewModel Priority { get; set; }

    [ObservableProperty]
    public partial ActionsViewModel Actions { get; set; }

    #endregion

    public StandardMatchViewModel(IServiceProvider serviceProvider, IMessenger messenger, IVibrateService vibrateService)
        : base(serviceProvider, messenger)
    {
        Settings = serviceProvider.GetRequiredService<StandardMatchSettingsViewModel>();
        Clock = new(Guid, Settings.TimeLimit, serviceProvider, messenger, vibrateService);
        Scores = new();
        Priority = new(Guid, Scores, messenger);
        Actions = new(Clock, Scores, Priority);

        if (!Design.IsDesignMode)
            messenger.RegisterAll(this, Guid);
    }

    protected override void SetInheritedModel(Match model)
    {
        if (model is StandardMatch standardMatch)
        {
            standardMatch.Clock = Clock.ToModel();
            standardMatch.Scores = Scores.ToModel();
            standardMatch.Priority = Priority.ToModel();
            standardMatch.Actions = [.. Actions.ToModel()];
        }
    }
}
