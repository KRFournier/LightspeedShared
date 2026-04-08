using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Lightspeed.Messages;

namespace Lightspeed.Dialogs;

public partial class SelectDialog : UserControl
{
    private readonly SelectDialogMessage? _message;
    private readonly System.Action? _closeAction;

    public SelectDialog()
    {
        InitializeComponent();
    }

    public SelectDialog(SelectDialogMessage message, System.Action onClose) : this()
    {
        _message = message;
        _closeAction = onClose;

        TitleTextBlock.Text = message.Title;

        var content = new ViewLocator().Build(message.Selector);
        content.DataContext = message.Selector;
        ContentControl.Content = content;

        message.Selector.ItemSelected += Item_Selected;

        // put focus into the first textbox
        foreach (var child in ContentControl.GetVisualChildren())
        {
            if (child is TextBox tb && tb.IsVisible && tb.IsEnabled)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    tb.Focus();
                    tb.SelectAll();
                }, DispatcherPriority.Input);
                break;
            }
        }
    }

    private void Item_Selected(object? sender, object item)
    {
        _message?.Selector.ItemSelected -= Item_Selected;
        _closeAction?.Invoke();
        _message?.Respond(item);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        _message?.Selector.ItemSelected -= Item_Selected;
        _closeAction?.Invoke();
        _message?.Respond(null);
    }

    private void UserControl_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Cancel_Click(sender, new RoutedEventArgs());
        }
    }
}