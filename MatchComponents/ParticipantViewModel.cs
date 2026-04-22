using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lightspeed.MatchComponents;

public abstract partial class ParticipantViewModel(Guid guid, IMessenger? messenger = null) : ObservableObject
{
    /// <summary>
    /// The participant's unique identifier.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    /// The player's current honor
    /// </summary>
    [ObservableProperty]
    public partial HonorViewModel Honor { get; set; } = new HonorViewModel(guid, messenger);

    /// <summary>
    /// The player's current penalties
    /// </summary>
    [ObservableProperty]
    public partial PenaltiesViewModel Penalties { get; set; } = new PenaltiesViewModel(guid, messenger);

    /// <summary>
    /// Gets the model for this participant
    /// </summary>
    public abstract Participant ToModel();

    /// <summary>
    /// Used by descending objects to set common properties on the model object
    /// </summary>
    protected void SetModelBaseProperties(Participant model)
    {
        model.Id = Guid;
        model.Honor = Honor.HonorAmount;
        model.Penalties = Penalties.ToModel();
    }
}
