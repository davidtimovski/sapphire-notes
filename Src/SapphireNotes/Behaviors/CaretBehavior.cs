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
        public static readonly AttachedProperty<int> CursorPositionProperty = AvaloniaProperty.RegisterAttached<CaretBehavior, Interactive, int>(
            "CursorPosition", default, false, BindingMode.OneTime, ValidateCursorPosition);

        private static int _index;

        private static int ValidateCursorPosition(Interactive element, int index)
        {
            _index = index;

            var textBox = element as TextBox;
            textBox.AttachedToVisualTree += SetTextBoxCaretIndexThenFocus;

            element.AddHandler(InputElement.PointerReleasedEvent, UpdateViewModelCursorPosition);

            return index;

            void UpdateViewModelCursorPosition(object s, RoutedEventArgs e)
            {
                var vm = textBox.DataContext as NoteViewModel;
                vm.CursorPosition = textBox.CaretIndex;
            }
        }

        private static void SetTextBoxCaretIndexThenFocus(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.CaretIndex = _index;
            textBox.Focus();
        }

        public static void SetCursorPosition(AvaloniaObject element, int value)
        {
            element.SetValue(CursorPositionProperty, value);
        }

        public static int GetCursorPosition(AvaloniaObject element)
        {
            return element.GetValue(CursorPositionProperty);
        }
    }
}
