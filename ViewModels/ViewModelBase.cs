using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Lightspeed.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Lightspeed.ViewModels;

public partial class ViewModelBase(IServiceProvider serviceProvider, IMessenger messenger) : ObservableObject
{
    #region Service Helpers

    /// <summary>
    /// Creates a new view model using the service provider. This is useful for creating child view models that need to use dependency injection.
    /// </summary>
    protected T New<T>() where T : class => serviceProvider.GetRequiredService<T>();

    #endregion

    #region Message Helpers

    protected void Send<TMessage>()
        where TMessage : class, new() => messenger.Send<TMessage>();

    protected void Send<TMessage>(TMessage message)
        where TMessage : class => messenger.Send(message);

    protected void Send<TMessage, TToken>(TMessage message, TToken token)
        where TMessage : class
        where TToken : IEquatable<TToken> => messenger.Send(message, token);

    #endregion

    #region Dialog Helpers

    /// <summary>
    /// Used by parent view models to set dragging state
    /// </summary>
    [ObservableProperty]
    public partial bool IsDragging { get; set; } = false;

    /// <summary>
    /// The result of a call to DialogBox
    /// </summary>
    public class DialogBoxResult<T> where T : ObservableObject
    {
        public bool IsOk { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public T Item { get; set; }
        public DialogBoxResult(DialogResponse response, T item)
        {
            switch (response)
            {
                case DialogResponse.Ok: IsOk = true; break;
                case DialogResponse.Cancel: IsCancelled = true; break;
            }
            Item = item;
        }
    }

    /// <summary>
    /// Closes the current dialog, if any.
    /// </summary>
    public void CloseDialog() => Send<CloseDialogMessage>();

    /// <summary>
    /// Opens an edit dialog with the given initial viewmodel and returns the result when the dialog is closed.
    /// </summary>
    public Task<DialogBoxResult<T>> EditDialog<T>(T initial, string title)
        where T : ObservableObject
    {
        var cs = new TaskCompletionSource<DialogBoxResult<T>>();
        EditDialogMessage message = new(initial, title, (vm, response) =>
        {
            if (vm is T item)
                cs.SetResult(new DialogBoxResult<T>(response, item));
            else
                throw new InvalidCastException($"Expected ViewModel of type {typeof(T).Name}, but got {vm.GetType().Name}.");
        });
        Send(message);
        return cs.Task;
    }

    /// <summary>
    /// Opens a selection dialog with the given initial viewmodel and returns the result when the dialog is closed.
    /// </summary>
    public Task<TItem> SelectDialog<TViewModel, TItem>(TViewModel initial, string title)
        where TViewModel : SelectionViewModelBase
    {
        var cs = new TaskCompletionSource<TItem>();
        SelectDialogMessage message = new(initial, title, (item) =>
        {
            if (item is TItem selectedItem)
                cs.SetResult(selectedItem);
            else
                throw new InvalidCastException($"Expected item of type {typeof(TItem).Name}, but got {item?.GetType().Name}.");
        });
        Send(message);
        return cs.Task;
    }

    /// <summary>
    /// Opens the login dialog and returns the result when the dialog is closed.
    /// </summary>
    public Task<DialogResponse> ShowLoginDialog()
    {
        var cs = new TaskCompletionSource<DialogResponse>();
        ShowLoginDialogMessage message = new(response => cs.SetResult(response));
        Send(message);
        return cs.Task;
    }

    /// <summary>
    /// Opens the signature dialog and returns the signature when the dialog is closed.
    /// </summary>
    public Task<string?> ShowSignDialog()
    {
        var cs = new TaskCompletionSource<string?>();
        ShowSignDialogMessage message = new(response => cs.SetResult(response));
        Send(message);
        return cs.Task;
    }

    /// <summary>
    /// Shows the message in a message box
    /// </summary>
    public Task MessageBox(string msg)
    {
        var cs = new TaskCompletionSource<bool>();
        MessageBoxMessage messageBoxMessage = new(msg, () => cs.SetResult(true));
        Send(messageBoxMessage);
        return cs.Task;
    }

    /// <summary>
    /// Shows a wait dialog
    /// </summary>
    public void BeginWait(string msg) => Send(new BeginWaitMessage(msg));

    /// <summary>
    /// Hides the wait dialog
    /// </summary>
    public void EndWait() => Send<EndWaitMessage>();

    #endregion
}
