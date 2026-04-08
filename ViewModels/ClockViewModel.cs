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
    public partial TimeSpan TimeStart { get; set; } = TimeSpan.FromMinutes(2);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTimeUp))]
    //[NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    //[NotifyPropertyChangedFor(nameof(IsRightWinner))]
    //[NotifyPropertyChangedFor(nameof(IsMatchCompleted))]
    public partial TimeSpan TimeRemaining { get; set; } = TimeSpan.FromSeconds(90);

    [ObservableProperty]
    public partial int Overtime { get; set; } = 0;

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
    /// Adds time to the clock
    /// </summary>
    [RelayCommand]
    private void AddTime()
    {
        if (IsStarted)
        {
            TimeRemaining += One;
        }
        else
        {
            TimeStart += Fifteen;
            TimeRemaining = TimeStart;
        }
        Send(new ClockUpdatedMessage(ToState()));
    }

    [RelayCommand]
    private void SubtractTime()
    {
        if (IsStarted)
        {
            if (TimeRemaining > One)
                TimeRemaining -= One;
            else
                TimeRemaining = TimeSpan.Zero;
            Send(new ClockUpdatedMessage(ToState()));
        }
        else
        {
            if (TimeStart > Fifteen)
            {
                TimeStart -= Fifteen;
                TimeRemaining = TimeStart;
                Send(new ClockUpdatedMessage(ToState()));
            }
        }
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    [RelayCommand]
    private void Fence()
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
    private void Break()
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

    public override string ToString() => $"{TimeRemaining:mm\\:ss}" + (Overtime > 0 ? $" +{Overtime}OT" : string.Empty);

    public Clock ToModel() => new()
    {
        Overtime = Overtime,
        Timer = TimeRemaining
    };

    public ClockState ToState() => new()
    {
        TimeRemaining = TimeRemaining,
        OvertimeCount = Overtime
    };

    public void UpdateState(ClockState? state)
    {
        if (state is null)
            return;

        TimeRemaining = state.TimeRemaining;
        Overtime = state.OvertimeCount;
    }
}
