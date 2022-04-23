using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace SapphireNotes.Behaviors;

public class FocusBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<bool> InitialFocusProperty = AvaloniaProperty.RegisterAttached<FocusBehavior, Interactive, bool>(
        "InitialFocus", default, false, BindingMode.OneTime);

    static FocusBehavior()
    {
        InitialFocusProperty.Changed.Subscribe(Observer.Create<AvaloniaPropertyChangedEventArgs>(e => {
            var textBox = e.Sender as TextBox;
            textBox.AttachedToVisualTree += TextBox_AttachedToVisualTree;
        }));
    }

    private static void TextBox_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        var textBox = sender as TextBox;
        textBox.Focus();
    }

    public static void SetInitialFocus(AvaloniaObject element, bool value)
    {
        element.SetValue(InitialFocusProperty, value);
    }

    public static bool GetInitialFocus(AvaloniaObject element)
    {
        return element.GetValue(InitialFocusProperty);
    }
}
