using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Behaviors
{
    public class CaretBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<int> CaretPositionProperty = AvaloniaProperty.RegisterAttached<CaretBehavior, Interactive, int>(
            "CaretPosition", default, false, BindingMode.OneTime, ValidateCaretPosition);

        private static int ValidateCaretPosition(Interactive element, int index)
        {
            var textBox = element as TextBox;
            textBox.AttachedToVisualTree += SetTextBoxCaretIndexThenFocus;

            element.AddHandler(InputElement.PointerReleasedEvent, UpdateViewModelCaretPosition);
            element.AddHandler(InputElement.KeyUpEvent, UpdateViewModelCaretPosition);

            return index;

            void SetTextBoxCaretIndexThenFocus(object sender, EventArgs e)
            {
                var textBox = sender as TextBox;
                textBox.CaretIndex = index;
                textBox.Focus();
            }

            void UpdateViewModelCaretPosition(object s, RoutedEventArgs e)
            {
                var vm = textBox.DataContext as NoteViewModel;
                vm.CaretPosition = textBox.CaretIndex;
            }
        }

        public static void SetCaretPosition(AvaloniaObject element, int value)
        {
            element.SetValue(CaretPositionProperty, value);
        }

        public static int GetCaretPosition(AvaloniaObject element)
        {
            return element.GetValue(CaretPositionProperty);
        }
    }
}
