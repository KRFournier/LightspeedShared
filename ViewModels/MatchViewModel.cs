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
public abstract partial class MatchViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    #region Properties

    public Guid Guid { get; set; } = Guid.NewGuid();

    [ObservableProperty]
    public partial int? Number { get; set; }

    [ObservableProperty]
    public partial bool IsLive { get; set; } = false;

    #endregion

    /// <summary>
    /// Gets the model for this match. Must be implemented by inheriting view models to return the appropriate match model type.
    /// </summary>
    public Match? ToModel()
    {
        if( this is MatchNotFoundViewModel)
            return null;

        var name = GetType().FullName!.Replace("ViewModel", "");
        var type = Type.GetType(name);
        if (type is null)
        {
            throw new InvalidOperationException($"Could not find type {name}");
        }
        else
        {
            var model = Activator.CreateInstance(type) as Match ?? throw new InvalidOperationException($"Could not create instance of type {type.FullName}");
            model.Id = Guid;
            model.Number = Number;
            SetInheritedModel(model);
            return model;
        }
    }

    /// <summary>
    /// Sets additional properties on the model that are specific to the derived class.
    /// </summary>
    protected abstract void SetInheritedModel(Match model);
}

/// <summary>
/// A placeholder for matches that could not be found
/// </summary>
public partial class MatchNotFoundViewModel : MatchViewModel
{
    public override string ToString() => "Match Not Found!";

    public MatchNotFoundViewModel(IServiceProvider serviceProvider, IMessenger messenger) : base(serviceProvider, messenger)
    {
        Guid = Guid.Empty;
    }

    protected override void SetInheritedModel(Match model) { }
}
