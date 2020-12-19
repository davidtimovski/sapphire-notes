using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SapphireNotes.Behaviors
{
    public class MiddleClickBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<MiddleClickBehavior, Interactive, ICommand>(
            "Command", default, false, BindingMode.OneTime, ValidateCommand);

        private static ICommand ValidateCommand(Interactive element, ICommand commandValue)
        {
            element.AddHandler(InputElement.PointerReleasedEvent, Handler);
            return commandValue;

            void Handler(object s, PointerReleasedEventArgs e)
            {
                if (e.GetCurrentPoint(element).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased
                    && commandValue?.CanExecute(null) == true)
                {
                    commandValue.Execute(null);
                }
            }
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
}
