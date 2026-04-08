using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Lightspeed.ViewModels;

/// <summary>
/// A participant(s)' score in a match
/// </summary>
public partial class ScoreViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    /// <summary>
    /// The participant to whome this score belongs. This will automatically be set if this score is linked to a parent match.
    /// </summary>
    [ObservableProperty]
    public partial ParticipantViewModel Participant { get; set; } = serviceProvider.GetRequiredService<EmptyParticipantViewModel>();
    partial void OnParticipantChanged(ParticipantViewModel oldValue, ParticipantViewModel newValue)
    {
        oldValue?.PropertyChanged -= OnParticipantPropertyChanged;
        newValue.PropertyChanged += OnParticipantPropertyChanged;
    }

    /// <summary>
    /// The participant's score in this match. The interpretation of this value is up to the match type.
    /// </summary>
    [ObservableProperty]
    public partial int Points { get; set; } = 0;

    /// <summary>
    /// The optional seed number for this participant. This is used for display purposes and has no effect on match outcomes.
    /// </summary>
    [ObservableProperty]
    public partial int? Seed { get; set; }

    /// <summary>
    /// The participants minor violations in this match.
    /// </summary>
    [ObservableProperty]
    public partial int MinorViolations { get; set; } = 0;

    /// <summary>
    /// Determines if this player/team is out of the match, either due to disqualification or
    /// because this score represents a bye
    /// </summary>
    public bool IsOut => Participant.IsDisqualified || Participant.IsBye;

    /// <summary>
    /// Optional reference to a parent match. If set, the participant of this score will
    /// utomatically be set to parent match's winner/loser.
    /// </summary>
    public MatchViewModel? ParentMatch { get; set; }

    /// <summary>
    /// Which outcome to use from the parent match. Ignored if ParentMatch is null.
    /// </summary>
    public MatchOutcome ParentMatchRef { get; set; } = MatchOutcome.Winner;

    #endregion

    protected void OnParticipantPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ParticipantViewModel.IsDisqualified))
            OnPropertyChanged(nameof(IsOut));
    }

    public Score ToModel() => new()
    {
        Participant = Participant.Guid,
        Points = Points,
        Seed = Seed,
        MinorViolations = MinorViolations
    };

    public override string ToString() => ParentMatch is not null && Participant.IsEmpty ? $"{ParentMatchRef} of ({ParentMatch})" : Participant.Name;

    public ScoreState ToState() => new()
    {
        Participant = Participant.ToState(),
        Points = Points,
        Seed = Seed,
        MinorViolationCount = MinorViolations
    };

    public void UpdateState(ScoreState? state)
    {
        if (state is null)
            return;

        Participant.UpdateState(state.Participant);
        Points = state.Points;
        Seed = state.Seed;
        MinorViolations = state.MinorViolationCount;
    }
}