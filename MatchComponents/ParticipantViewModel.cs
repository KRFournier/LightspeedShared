using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lightspeed.MatchComponents;

public abstract partial class ParticipantViewModel(Guid guid, IMessenger messenger) : ObservableObject
{
    /// <summary>
    /// The participant's unique identifier.
    /// </summary>
    public Guid Guid { get; } = guid;

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
    /// The optional seed number for this player. This is used for display purposes and has no effect on match outcomes.
    /// </summary>
    [ObservableProperty]
    public partial int? Seed { get; set; }

    /// <summary>
    /// Exports this view model to a Participant model. This method uses reflection to find the corresponding Participant type based on the name of the view model class.
    /// It then creates an instance of that type and populates it with the properties from this view model.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Participant ToModel()
    {
        var name = GetType().FullName!.Replace("ViewModel", "");
        var type = Type.GetType(name);
        if (type is null)
        {
            throw new InvalidOperationException($"Could not find type {name}");
        }
        else
        {
            var model = Activator.CreateInstance(type) as Participant ?? throw new InvalidOperationException($"Could not create instance of type {type.FullName}");
            model.Id = Guid;
            model.Honor = Honor.HonorAmount;
            model.Penalties = Penalties.ToModel();
            model.Seed = Seed;
            SetInheritedModel(model);
            return model;
        }
    }

    /// <summary>
    /// Sets additional properties on the model that are specific to the derived class.
    /// </summary>
    protected abstract void SetInheritedModel(Participant model);
}
