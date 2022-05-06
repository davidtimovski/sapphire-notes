using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SapphireNotes.Behaviors;

public class MiddleClickBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<MiddleClickBehavior, Interactive, ICommand>(
        "Command", default, false, BindingMode.OneTime);

    static MiddleClickBehavior()
    {
        CommandProperty.Changed.Subscribe(Observer.Create<AvaloniaPropertyChangedEventArgs>(e => {
            var element = e.Sender as Border;
            element.AddHandler(InputElement.PointerReleasedEvent, Handler);

            void Handler(object s, PointerReleasedEventArgs pointerReleasedEvent)
            {
                if (pointerReleasedEvent.GetCurrentPoint(element).Properties.PointerUpdateKind !=
                    PointerUpdateKind.MiddleButtonReleased) return;
                var commandValue = e.NewValue as ICommand;
                if (commandValue?.CanExecute(null) == true)
                {
                    commandValue.Execute(null);
                }
            }
        }));
    }

    public static void SetCommand(AvaloniaObject element, ICommand commandValue)
    {
        element.SetValue(CommandProperty, commandValue);
    }

    public static ICommand GetCommand(AvaloniaObject element)
    {
        return element.GetValue(CommandProperty);
    }
}
