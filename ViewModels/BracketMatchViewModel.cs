using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Lightspeed.ViewModels;

/// <summary>
/// A match in which each side is seeded
/// </summary>
public partial class BracketMatchViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    [ObservableProperty]
    public partial MatchViewModel Match { get; set; }

    [ObservableProperty]
    public partial int LeftSeed { get; set; } = 0;

    [ObservableProperty]
    public partial int RightSeed { get; set; } = 0;
}
