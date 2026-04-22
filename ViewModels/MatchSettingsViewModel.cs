using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Network;

namespace Lightspeed.ViewModels;

/// <summary>
/// Settings for a group of matches
/// </summary>
public partial class MatchSettingsViewModel(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    /// <summary>
    /// Set this to true to lock the user from making changes, typically
    /// after a match in a group has started
    /// </summary>
    [ObservableProperty]
    public partial bool IsLocked { get; set; } = false;

    /// <summary>
    /// Creates a copy that can be edited without affecting the original
    /// </summary>
    public virtual MatchSettingsViewModel Clone() => New<MatchSettingsViewModel>();

    /// <summary>
    /// Exports this view model to a model.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public MatchSettings ToModel()
    {
        var name = GetType().FullName!.Replace("ViewModel", "");
        var type = Type.GetType(name);
        if (type is null)
        {
            throw new InvalidOperationException($"Could not find type {name}");
        }
        else
        {
            var model = Activator.CreateInstance(type) as MatchSettings ?? throw new InvalidOperationException($"Could not create instance of type {type.FullName}");
            model.IsLocked = IsLocked;
            SetInheritedModel(model);
            return model;
        }
    }

    /// <summary>
    /// Sets additional properties on the model that are specific to the derived class.
    /// </summary>
    protected virtual void SetInheritedModel(MatchSettings model) { }
}
