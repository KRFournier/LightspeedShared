using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Lightspeed.Services;

namespace Lightspeed.ViewModels;

#region Messages

public sealed class ClockUpdatedMessage(ClockState state)
{
    public ClockState State { get; } = state;
}

#endregion

/// <summary>
/// A clock is a set of one (or more) timers with support for overtime.
/// The match ends if any timer reaches zero.
/// </summary>
public partial class ClockViewModel(IServiceProvider serviceProvider, IMessenger messenger, IVibrateService vibrateService) : ViewModelBase(serviceProvider, messenger)
{
    private readonly System.Timers.Timer timer = new();
    private static readonly TimeSpan One = new(0, 0, 1);
    private static readonly TimeSpan Fifteen = new(0, 0, 15);

    #region Properties

    /// <summary>
    /// The initial time
    /// </summary>
    [ObservableProperty]
    public partial TimeSpan TimeStart { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// The current time remaining. If this reaches zero, the match is over.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTimeUp))]
    public partial TimeSpan TimeRemaining { get; set; } = TimeSpan.FromSeconds(90);

    /// <summary>
    /// The current round
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMultipleRounds))]
    [NotifyPropertyChangedFor(nameof(IsOvertime))]
    [NotifyPropertyChangedFor(nameof(Status))]
    public partial int CurrentRound { get; set; } = 1;

    /// <summary>
    /// The total number of rounds
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMultipleRounds))]
    [NotifyPropertyChangedFor(nameof(IsOvertime))]
    [NotifyPropertyChangedFor(nameof(Status))]
    public partial int TotalRounds { get; set; } = 1;

    /// <summary>
    /// Determines if the match has multiple rounds.
    /// </summary>
    public bool HasMultipleRounds => TotalRounds > 1;

    /// <summary>
    /// Determines if we're in overtime
    /// </summary>
    public bool IsOvertime => CurrentRound > TotalRounds;

    /// <summary>
    /// The status of the clock, which can be "Round X of Y", "Overtime", "Priority Overtime", or "Match Over".
    /// </summary>
    public string? Status
    {
        get
        {
            if (CurrentRound == TotalRounds + 1)
                return "Overtime";
            else if (CurrentRound == TotalRounds + 2)
                return "Priority Overtime";
            else if (TotalRounds > 1 && CurrentRound <= TotalRounds)
                return $"Round {CurrentRound} of {TotalRounds}";
            else
                return null;
        }
    }

    /// <summary>
    /// Determines if the time is up
    /// </summary>
    public bool IsTimeUp => TimeRemaining <= TimeSpan.Zero;

    /// <summary>
    /// Determines if the match has started
    /// </summary>
    [ObservableProperty]
    public partial bool IsStarted { get; set; } = false;

    /// <summary>
    /// Indicates whether we are fencing!
    /// </summary>
    [ObservableProperty]
    public partial bool IsRunning { get; set; } = false;

    #endregion

    #region Commands

    /// <summary>
    /// Adds one second
    /// </summary>
    [RelayCommand]
    public void AddSecond()
    {
        TimeRemaining += One;
        Send(new ClockUpdatedMessage(ToState()));
    }

    /// <summary>
    /// Subtracts one second
    /// </summary>
    [RelayCommand]
    public void SubtractSecond()
    {
        if (TimeRemaining > One)
            TimeRemaining -= One;
        else
            TimeRemaining = TimeSpan.Zero;
        Send(new ClockUpdatedMessage(ToState()));
    }

    /// <summary>
    /// Adds 15 seconds
    /// </summary>
    [RelayCommand]
    public void AddQuarter()
    {
        TimeStart += Fifteen;
        TimeRemaining = TimeStart;
        Send(new ClockUpdatedMessage(ToState()));
    }

    /// <summary>
    /// Subtracts 15 seconds
    /// </summary>
    [RelayCommand]
    public void SubtractQuarter()
    {
        if (TimeStart > Fifteen)
        {
            TimeStart -= Fifteen;
            TimeRemaining = TimeStart;
            Send(new ClockUpdatedMessage(ToState()));
        }
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    [RelayCommand]
    public void Start()
    {
        if (!IsTimeUp)
        {
            timer.Start();
            IsStarted = true;
            IsRunning = true;
            Send(new ClockUpdatedMessage(ToState()));
        }
    }

    /// <summary>
    /// Stops the timer
    /// </summary>
    [RelayCommand]
    public void Break()
    {
        timer.Stop();
        vibrateService?.Stop();
        IsRunning = false;
        Send(new ClockUpdatedMessage(ToState()));
    }

    /// <summary>
    /// Handles the timer tick
    /// </summary>
    private void OnTimerTick(object? source, System.Timers.ElapsedEventArgs e)
    {
        TimeRemaining -= One;
        if (TimeRemaining <= TimeSpan.Zero)
        {
            timer.Stop();
            Send(new ClockUpdatedMessage(ToState()));
            vibrateService?.Start();
        }
    }

    #endregion

    public override string ToString() => $"{TimeRemaining:mm\\:ss}, Round {CurrentRound}";

    public Clock ToModel() => new()
    {
        CurrentRound = CurrentRound,
        Timer = TimeRemaining
    };

    public ClockState ToState() => new()
    {
        TimeRemaining = TimeRemaining,
        CurrentRound = CurrentRound
    };

    public void UpdateState(ClockState? state)
    {
        if (state is null)
            return;

        TimeRemaining = state.TimeRemaining;
        CurrentRound = state.CurrentRound;
    }
}
