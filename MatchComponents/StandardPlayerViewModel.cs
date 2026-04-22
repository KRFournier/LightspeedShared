using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Lightspeed.MatchComponents;

/// <summary>
/// A standard player is a single competitor in a tournement with tournament-wide penalties and honor.
/// </summary>
public sealed partial class StandardPlayerViewModel(CompetitorViewModel competitor, IMessenger messenger) : ParticipantViewModel(competitor.Guid, messenger)
{
    /// <summary>
    /// The competitor associated with this player. This is the core data for the player, including name, club, and weapon of choice.
    /// </summary>
    [ObservableProperty]
    public partial CompetitorViewModel Competitor { get; set; } = competitor;

    /// <summary>
    /// Sets additional properties on the model that are specific to this class.
    /// </summary>
    protected override void SetInheritedModel(Participant model)
    {
        if (model is StandardPlayer standardPlayer)
        {
            standardPlayer.Competitor = Competitor.ToModel();
        }
    }
}
