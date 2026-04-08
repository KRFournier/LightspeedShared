using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;

namespace Lightspeed.ViewModels;

/// <summary>
/// Settings for a group of matches
/// </summary>
public partial class MatchSettingsViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    /// <summary>
    /// The points needed to win a match in this pool
    /// </summary>
    [ObservableProperty]
    public partial int WinningScore { get; set; } = 12;

    /// <summary>
    /// The time limit for matches in this pool
    /// </summary>
    [ObservableProperty]
    public partial TimeSpan TimeLimit { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// The number of rounds into which a match is divided
    /// </summary>
    [ObservableProperty]
    public partial int Rounds { get; set; } = 1;

    /// <summary>
    /// Set this to true to lock the user from making changes, typically
    /// after a match in a group has started
    /// </summary>
    [ObservableProperty]
    public partial bool IsLocked { get; set; } = false;

    #endregion

    #region Commands

    [RelayCommand]
    private void DecreaseAll()
    {
        int chunk = WinningScore / 4;
        if (chunk-- > 0)
        {
            WinningScore = chunk * 4;
            TimeLimit = TimeSpan.FromSeconds(chunk * 30);
        }
    }

    [RelayCommand]
    private void IncreaseAll()
    {
        int chunk = WinningScore / 4 + 1;
        WinningScore = chunk * 4;
        TimeLimit = TimeSpan.FromSeconds(chunk * 30);
    }

    [RelayCommand]
    private void IncreaseScore() => WinningScore++;

    [RelayCommand]
    private void DecreaseScore()
    {
        if (WinningScore > 0)
            WinningScore--;
    }

    [RelayCommand]
    private void IncreaseTime() => TimeLimit += TimeSpan.FromSeconds(15);

    [RelayCommand]
    private void DecreaseTime()
    {
        if (TimeLimit.TotalSeconds > 15)
            TimeLimit -= TimeSpan.FromSeconds(15);
    }

    [RelayCommand]
    private void IncreaseRounds() => Rounds++;

    [RelayCommand]
    private void DecreaseRounds()
    {
        if (Rounds > 1)
            Rounds--;
    }

    #endregion

    /// <summary>
    /// Creates a copy that can be edited without affecting the original
    /// </summary>
    public MatchSettingsViewModel Clone()
    {
        var settings = New<MatchSettingsViewModel>();
        settings.WinningScore = WinningScore;
        settings.TimeLimit = TimeLimit;
        settings.Rounds = Rounds;
        return settings;
    }

    public MatchSettings ToModel() => new()
    {
        WinningScore = WinningScore,
        TimeLimit = TimeLimit,
        Rounds = Rounds,
        IsLocked = IsLocked
    };

    public MatchSettingsState ToState() => new()
    {
        WinningScore = WinningScore,
        TimeLimit = TimeLimit,
        Rounds = Rounds
    };
}
