using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Lightspeed.ViewModels;

namespace Lightspeed.Messages;

/// <summary>
/// Possible responses from the dialog
/// </summary>
public enum DialogResponse
{
    None,
    Ok,
    Cancel
}

/// <summary>
/// Notifies the main view that the given viewmodel should be opened as a dialog for editing.
/// Users must pass a copy of the viewmodel to be edited, which will be returned with changes.
/// It's up to the user to keep or discard the changes based on the response.
/// </summary>
public class EditDialogMessage(
    ObservableObject item,
    string title,
    Action<ObservableObject, DialogResponse> handler
    )
{

    /// <summary>
    /// The initial content for the dialog
    /// </summary>
    public readonly ObservableObject Item = item;

    /// <summary>
    /// The dialog box title
    /// </summary>
    public readonly string Title = title;

    /// <summary>
    /// Final response from the dialog
    /// </summary>
    private readonly Action<ObservableObject, DialogResponse> _handler = handler;

    /// <summary>
    /// Respond to the caller
    /// </summary>
    public void Respond(DialogResponse response) => _handler(Item, response);
}

/// <summary>
/// Notifies the main view that the given viewmodel should be opened as a dialog for editing.
/// Users must pass a copy of the viewmodel to be edited, which will be returned with changes.
/// It's up to the user to keep or discard the changes based on the response.
/// </summary>
public class SelectDialogMessage(
    SelectionViewModelBase selector,
    string title,
    Action<object?> handler
    )
{
    /// <summary>
    /// The initial content for the dialog
    /// </summary>
    public readonly SelectionViewModelBase Selector = selector;

    /// <summary>
    /// The dialog box title
    /// </summary>
    public readonly string Title = title;

    /// <summary>
    /// Final response from the dialog
    /// </summary>
    private readonly Action<object?> _handler = handler;

    /// <summary>
    /// Respond to the caller
    /// </summary>
    public void Respond(object? item) => _handler(item);
}

/// <summary>
/// Notifies the main view to close the current dialog
/// </summary>
public class CloseDialogMessage()
{
}

/// <summary>
/// Notifies the main view that the given message should be displayed
/// </summary>
public class MessageBoxMessage(string msg, System.Action? handler = null) : ValueChangedMessage<string>(msg)
{
    private readonly System.Action? _handler = handler;
    public void Respond() => _handler?.Invoke();
}

/// <summary>
/// Notifies the main view that the given wait message should be displayed
/// </summary>
public class BeginWaitMessage(string msg) : ValueChangedMessage<string>(msg)
{
}

/// <summary>
/// Notifies the main view that the given wait message should be hidden
/// </summary>
public class EndWaitMessage() { }

/// <summary>
/// Notifies the main view that the login dialog should be displayed, and allows
/// for a response to be sent back.
/// </summary>
public class ShowLoginDialogMessage(Action<DialogResponse> handler)
{
    private readonly Action<DialogResponse> _handler = handler;
    public void Respond(DialogResponse response) => _handler(response);
}

/// <summary>
/// Notifies the main view that the signature dialog should be displayed, and allows
/// for the signature to be sent back.
/// </summary>
public class ShowSignDialogMessage(Action<string?> handler)
{
    private readonly Action<string?> _handler = handler;
    public void Respond(string? response) => _handler(response);
}
