using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Lightspeed.Network;
using Lightspeed.Network.Messages;
using System.ComponentModel;

namespace Lightspeed.ViewModels;

#region Messages

public sealed class MatchWinnerChangedMessage(MatchViewModel match) : ValueChangedMessage<MatchViewModel>(match) { }

#endregion

/// <summary>
/// The base class for all matches
/// </summary>
public abstract partial class MatchViewModel : ViewModelBase,
    IRecipient<RequestMatchState>, IRecipient<SetLiveMessage>, IRecipient<PriorityChangedMessage>
{
    #region Properties

    public Guid Guid { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    public partial MatchSettingsViewModel Settings { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBye))]
    [NotifyPropertyChangedFor(nameof(HasFirst))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(Left))]
    [NotifyPropertyChangedFor(nameof(Right))]
    [NotifyPropertyChangedFor(nameof(IsFirstWinner))]
    [NotifyPropertyChangedFor(nameof(IsSecondWinner))]
    [NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    [NotifyPropertyChangedFor(nameof(IsRightWinner))]
    [NotifyPropertyChangedFor(nameof(FirstHasPriority))]
    [NotifyPropertyChangedFor(nameof(SecondHasPriority))]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    public partial ScoreViewModel First { get; set; }
    partial void OnFirstChanged(ScoreViewModel oldValue, ScoreViewModel newValue)
    {
        oldValue?.PropertyChanged -= ScorePropertyChanged;
        newValue?.PropertyChanged += ScorePropertyChanged;
    }
    public bool HasFirst => !First.Participant.IsEmpty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBye))]
    [NotifyPropertyChangedFor(nameof(HasSecond))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    [NotifyPropertyChangedFor(nameof(Left))]
    [NotifyPropertyChangedFor(nameof(Right))]
    [NotifyPropertyChangedFor(nameof(IsFirstWinner))]
    [NotifyPropertyChangedFor(nameof(IsSecondWinner))]
    [NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    [NotifyPropertyChangedFor(nameof(IsRightWinner))]
    [NotifyPropertyChangedFor(nameof(FirstHasPriority))]
    [NotifyPropertyChangedFor(nameof(SecondHasPriority))]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    public partial ScoreViewModel Second { get; set; }
    partial void OnSecondChanged(ScoreViewModel oldValue, ScoreViewModel newValue)
    {
        oldValue?.PropertyChanged -= ScorePropertyChanged;
        newValue?.PropertyChanged += ScorePropertyChanged;
    }
    public bool HasSecond => !Second.Participant.IsEmpty;

    [ObservableProperty]
    public partial bool IsMatchStarted { get; set; } = false;

    [ObservableProperty]
    public partial int? Number { get; set; }

    [ObservableProperty]
    public partial bool IsLive { get; set; } = false;

    public bool HasBye => First.Participant.IsBye || Second.Participant.IsBye;

    public bool IsEmpty => First.Participant is EmptyParticipantViewModel && Second.Participant is EmptyParticipantViewModel;

    #endregion

    #region MessageHandlers

    public void Receive(RequestMatchState message)
    {
        message.Reply(ToState());
    }

    public void Receive(SetLiveMessage message)
    {
        IsLive = message.IsLive;
    }

    public void Receive(PriorityChangedMessage message)
    {
        if (message.State.Priority is null)
            return;

        PrioritySide = message.State.Priority.PrioritySide;
        PriorityPoints = message.State.Priority.PriorityPoints;
        InPriority = message.State.Priority.InPriority;
    }

    #endregion

    public MatchViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Settings = New<MatchSettingsViewModel>();
        First = New<ScoreViewModel>();
        Second = New<ScoreViewModel>();

        if (!Design.IsDesignMode)
        {
            messenger.Register<RequestMatchState, Guid>(this, Guid);
            messenger.Register<SetLiveMessage, Guid>(this, Guid);
            messenger.Register<PriorityChangedMessage, Guid>(this, Guid);
        }
    }

    /// <summary>
    /// Gets the model for this match. Must be implemented by inheriting view models to return the appropriate match model type.
    /// </summary>
    /// <returns></returns>
    public abstract Match? ToModel();

    /// <summary>
    /// Called by inheriting view models to set the common match model fields
    /// </summary>
    protected void UpdateMatchModel(Match model)
    {
        model.Id = Guid;
        model.Number = Number;
        model.First = First.ToModel();
        model.Second = Second.ToModel();
        model.Winner = WinningSide;
        model.IsMatchStarted = IsMatchStarted;
        model.Priority = new()
        {
            PrioritySide = Side.Neither,
            PriorityPoints = 3,
            InPriority = false
        };
    }

    /// <summary>
    /// Gets the state for this match. Must be implemented by inheriting view models to return the appropriate match state type.
    /// </summary>
    public abstract MatchState ToState();

    /// <summary>
    /// Gets the match summary for this match.
    /// </summary>
    /// <returns></returns>
    public MatchSummary ToSummary() => new()
    {
        Id = Guid,
        Number = Number ?? 0,
        First = First.ToState(),
        Second = Second.ToState(),
        Winner = WinningSide,
        IsStarted = IsMatchStarted,
        IsCompleted = IsMatchCompleted,
        Clock = GetClockState()
    };

    /// <summary>
    /// Allows the inheriting class a chance to provide the state of the clock
    /// </summary>
    protected virtual ClockState? GetClockState() => null;

    /// <summary>
    /// Updates a match's points. Usually called by the app's match editing screen
    /// </summary>
    public void UpdateMatch(int firstPoints, int secondPoints)
    {
        First.Points = firstPoints;
        Second.Points = secondPoints;
        IsMatchStarted = true;
        if (First.Points != Second.Points)
            Winner = First.Points > Second.Points ? First : Second;
    }

    /// <summary>
    /// Track changes in the participants so we can updates some properties
    /// </summary>
    protected void ScorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Score.Participant))
        {
            OnPropertyChanged(nameof(HasBye));
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    /// <summary>
    /// Determines if the given participant is in this match
    /// </summary>
    public bool Contains(ParticipantViewModel participant) => First.Participant == participant || Second.Participant == participant;

    #region Winner

    /// <summary>
    /// The winner of the match, if there is one
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Loser))]
    [NotifyPropertyChangedFor(nameof(IsMatchCompleted))]
    [NotifyPropertyChangedFor(nameof(WinnerScore))]
    [NotifyPropertyChangedFor(nameof(LoserScore))]
    [NotifyPropertyChangedFor(nameof(IsFirstWinner))]
    [NotifyPropertyChangedFor(nameof(IsSecondWinner))]
    [NotifyPropertyChangedFor(nameof(WinningSide))]
    [NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    [NotifyPropertyChangedFor(nameof(IsRightWinner))]
    public partial ScoreViewModel? Winner { get; set; } = null;
    partial void OnWinnerChanged(ScoreViewModel? value) => Send(new MatchWinnerChangedMessage(this));

    /// <summary>
    /// The winner referenced by position in the match
    /// </summary>
    public Side WinningSide
    {
        get => Winner switch
        {
            _ when Winner == First => Side.First,
            _ when Winner == Second => Side.Second,
            _ => Side.Neither
        };

        set => Winner = value switch
        {
            Side.First => First,
            Side.Second => Second,
            _ => null
        };
    }

    /// <summary>
    /// The loser of the match
    /// </summary>
    public ScoreViewModel? Loser
    {
        get
        {
            if (Winner is not null)
            {
                if (Winner == First)
                    return Second;
                else if (Winner == Second)
                    return First;
            }
            return null;
        }
    }

    /// <summary>
    /// Determines if the winner is first
    /// </summary>
    public bool IsFirstWinner => Winner == First;

    /// <summary>
    /// Determines if the winner is red
    /// </summary>
    public bool IsSecondWinner => Winner == Second;

    /// <summary>
    /// The winner's score
    /// </summary>
    public int WinnerScore => IsFirstWinner ? First.Points : (IsSecondWinner ? Second.Points : 0);

    /// <summary>
    /// The loser's score
    /// </summary>
    public int LoserScore => IsFirstWinner ? Second.Points : (IsSecondWinner ? First.Points : 0);

    /// <summary>
    /// Determines if the match is completed based on whether or not there is a winner
    /// </summary>
    public virtual bool IsMatchCompleted => Winner is not null;

    /// <summary>
    /// Checks for a bye, and if there is one, sets the winner accordingly.
    /// If there is no bye, clears the winner to allow for normal match resolution
    /// </summary>
    public void CheckByeWinner()
    {
        if (First.Participant.IsBye && !Second.Participant.IsBye)
        {
            Winner = Second;
            return;
        }
        else if (!First.Participant.IsBye && Second.Participant.IsBye)
        {
            Winner = First;
            return;
        }

        Winner = null;
    }

    #endregion

    #region Priority

    // used to assign random priority in the case of a coin flip
    private readonly Random _random = new();

    /// <summary>
    /// The points to assign a player who scores a priority action.
    /// Currently set to 3, but may be changed in the future if we want to adjust how priority actions affect the match.
    /// </summary>
    [ObservableProperty]
    public partial int PriorityPoints { get; set; } = PointValues.Priority;

    /// <summary>
    /// The side with priority. Null if priority has not been assigned.
    /// Does not get cleared! We need to know who had priority last so we can alternate.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FirstHasPriority))]
    [NotifyPropertyChangedFor(nameof(SecondHasPriority))]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    public partial ScoreViewModel? Priority { get; set; }

    /// <summary>
    /// Determines if priority is active
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FirstHasPriority))]
    [NotifyPropertyChangedFor(nameof(SecondHasPriority))]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    public partial bool InPriority { get; set; } = false;

    /// <summary>
    /// Determines if First has priority
    /// </summary>
    public bool FirstHasPriority => InPriority && First == Priority;

    /// <summary>
    /// Determines if second has priority
    /// </summary>
    public bool SecondHasPriority => InPriority && Second == Priority;

    /// <summary>
    /// The priority participant referenced by position in the match
    /// </summary>
    public Side PrioritySide
    {
        get => Priority switch
        {
            _ when Priority == First => Side.First,
            _ when Priority == Second => Side.Second,
            _ => Side.Neither
        };
        set => Priority = value switch
        {
            Side.First => First,
            Side.Second => Second,
            _ => null
        };
    }

    /// <summary>
    /// Assigns priority using Honor if possible
    /// </summary>
    [RelayCommand]
    private void AssignPriority()
    {
        if (First is null || Second is null || InPriority || IsMatchCompleted)
            return;

        if (Priority is null)
        {
            if ((First.Participant.CurrentPlayer?.Honor ?? 0) > (Second.Participant.CurrentPlayer?.Honor ?? 0))
                Priority = First;
            else if ((First.Participant.CurrentPlayer?.Honor ?? 0) < (Second.Participant.CurrentPlayer?.Honor ?? 0))
                Priority = Second;
            else
                Priority = _random.Next(2) == 0 ? First : Second;
        }
        else
        {
            if (Priority == First)
                Priority = Second;
            else
                Priority = First;
        }

        InPriority = true;
    }

    /// <summary>
    /// Switches priority to the other player.
    /// </summary>
    [RelayCommand]
    private void SwapPriority()
    {
        if (FirstHasPriority)
            Priority = Second;
        else if (SecondHasPriority)
            Priority = First;
    }

    ///// <summary>
    ///// Should only be called when there is priority. If so, adds a Priority action.
    ///// We pass 0 points because <see cref="AddAction"/> will apply priority and
    ///// rescind it.
    ///// </summary>
    //[RelayCommand]
    //private void Simultaneous()
    //{
    //    if (InPriority && Priority is not null)
    //        AddAction(new(ActionType.Priority, Priority, PointValues.Priority));
    //}

    #endregion

    #region Player Position

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Left))]
    [NotifyPropertyChangedFor(nameof(Right))]
    [NotifyPropertyChangedFor(nameof(IsLeftWinner))]
    [NotifyPropertyChangedFor(nameof(IsRightWinner))]
    [NotifyPropertyChangedFor(nameof(LeftHasPriority))]
    [NotifyPropertyChangedFor(nameof(RightHasPriority))]
    public partial bool IsFirstLeft { get; set; } = true;

    public ScoreViewModel Left => IsFirstLeft ? First : Second;

    public ScoreViewModel Right => IsFirstLeft ? Second : First;

    public bool IsLeftWinner => IsFirstLeft ? IsFirstWinner : IsSecondWinner;

    public bool IsRightWinner => IsFirstLeft ? IsSecondWinner : IsFirstWinner;

    public bool LeftHasPriority => InPriority && Left == Priority;

    public bool RightHasPriority => InPriority && Right == Priority;

    [RelayCommand]
    private void SwapSides() => IsFirstLeft = !IsFirstLeft;

    #endregion

    public override string ToString() => Number is not null ?
        $"#{Number}: {First} vs {Second}" :
        $"{First} vs {Second}";
}

/// <summary>
/// A placeholder for matches that could not be found
/// </summary>
public partial class MatchNotFoundViewModel : MatchViewModel
{
    public override Match? ToModel() => null;
    public override bool IsMatchCompleted => true;
    public override string ToString() => "Match Not Found!";
    public override MatchState ToState() => throw new NotImplementedException();
    public MatchNotFoundViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Guid = Guid.Empty;
    }
}
