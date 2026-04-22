using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lightspeed.MatchComponents;

/// <summary>
/// Represents one side of a two-sided match. This tracks the participant, their score, and any minor violations they have received.
/// </summary>
public partial class SideViewModel : ObservableObject
{
    #region Properties

    /// <summary>
    /// The participant to whom this score belongs. This will automatically be set if this score is linked to a parent match.
    /// </summary>
    [ObservableProperty]
    public partial ParticipantViewModel Participant { get; set; }

    /// <summary>
    /// The participant's score in this match.
    /// </summary>
    [ObservableProperty]
    public partial int Points { get; set; } = 0;

    /// <summary>
    /// The participant's minor violations in this match.
    /// </summary>
    [ObservableProperty]
    public partial int MinorViolations { get; set; } = 0;

    #endregion

    #region Commands

    /// <summary>
    /// Adds a minor violation to this participant.
    /// </summary>
    [RelayCommand]
    public void GiveMinorViolation() => MinorViolations++;

    #endregion

    public Side ToModel() => new()
    {
        Participant = Participant.Guid,
        Points = Points,
        MinorViolations = MinorViolations
    };

    public override string ToString() => $"{Participant}, {Points}pts, {MinorViolations} minor violations";
}