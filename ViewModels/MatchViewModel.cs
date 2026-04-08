using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
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
public abstract partial class MatchViewModel : ViewModelBase
{
    #region Properties

    public Guid Guid { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    public partial MatchSettingsViewModel Settings { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBye))]
    [NotifyPropertyChangedFor(nameof(HasFirst))]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
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

    public MatchViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Settings = New<MatchSettingsViewModel>();
        First = New<ScoreViewModel>();
        Second = New<ScoreViewModel>();

        if (!Design.IsDesignMode)
        {
            // Listen for the network's request for this match's state
            messenger.Register<RequestMatchState, Guid>(this, Guid,
                (_, m) =>
                {
                    MatchState matchState = ToState();
                    matchState.Next = m.Next;
                    m.Reply(matchState);
                }
            );

            // Listen for live status updates
            messenger.Register<SetLiveMessage, Guid>(this, Guid, (_, m) => IsLive = m.IsLive);
        }
    }

    public abstract Match? ToModel();

    public virtual MatchState ToState() => throw new NotImplementedException();

    public virtual MatchSummary ToSummary() => throw new NotImplementedException();

    public void UpdateMatch(int firstPoints, int secondPoints)
    {
        First.Points = firstPoints;
        Second.Points = secondPoints;
        IsMatchStarted = true;
        if (First.Points != Second.Points)
            Winner = First.Points > Second.Points ? First : Second;
    }

    protected void ScorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Score.Participant))
        {
            OnPropertyChanged(nameof(HasBye));
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    /// <summary>
    /// Determines if the given participant is in in this match
    /// </summary>
    public bool Contains(ParticipantViewModel participant) => First.Participant == participant || Second.Participant == participant;

    #region Winner

    ///// <summary>
    ///// The match slot to which the winner of this match advances, if applicable
    ///// </summary>
    //[ObservableProperty]
    //public partial ScoreViewModel? WinnerAdvancement { get; set; }

    ///// <summary>
    ///// The match slot to which the loser of this match advances, if applicable
    ///// </summary>
    //[ObservableProperty]
    //public partial ScoreViewModel? LoserAdvancement { get; set; }

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
    public partial ScoreViewModel? Winner { get; set; } = null;
    partial void OnWinnerChanged(ScoreViewModel? value)
    {
        Send(new MatchWinnerChangedMessage(this));
        //if (WinnerAdvancement is not null)
        //{
        //    WinnerAdvancement.Participant = Winner?.Participant ?? New<EmptyParticipantViewModel>();
        //    WinnerAdvancement.Seed = Winner?.Seed;
        //}

        //if (LoserAdvancement is not null)
        //{
        //    LoserAdvancement.Participant = Loser?.Participant ?? New<EmptyParticipantViewModel>();
        //    LoserAdvancement.Seed = Loser?.Seed;
        //}
    }

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
    public MatchNotFoundViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Guid = Guid.Empty;
    }
}
