using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Lightspeed.MatchComponents;

/// <summary>
/// A single competitor in a tournament with tournament-wide penalties and honor.
/// </summary>
public sealed partial class SinglePlayerViewModel(CompetitorViewModel competitor, IMessenger messenger)
    : ParticipantViewModel(competitor.Guid, messenger)
{
    /// <summary>
    /// The competitor associated with this player. This is the core data for the player, including name, club, and weapon of choice.
    /// </summary>
    [ObservableProperty]
    public partial CompetitorViewModel Competitor { get; set; } = competitor;

    /// <summary>
    /// Gets the model for this player
    /// </summary>
    public override SinglePlayer ToModel()
    {
        var model = new SinglePlayer();
        SetModelBaseProperties(model);
        model.Competitor = Competitor.Guid;
        return model;
    }
}
