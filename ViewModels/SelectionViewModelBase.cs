using CommunityToolkit.Mvvm.Messaging;

namespace Lightspeed.ViewModels;

/// <summary>
/// Base class for viewmodels that allow the user to select an item from a list of items.
/// This is used for the select dialog.
/// </summary>
public class SelectionViewModelBase(IServiceProvider serviceProvider, IMessenger messenger) : ViewModelBase(serviceProvider, messenger)
{
    /// <summary>
    /// The event that is fired when the user selects an item.
    /// </summary>
    public event EventHandler<object>? ItemSelected;

    /// <summary>
    /// Notifies listeners that a selection has been made. Ineheriting view models must call this method.
    /// </summary>
    protected void NotifySelected(object item) => ItemSelected?.Invoke(this, item);
}
