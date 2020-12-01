using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SapphireNotes.Behaviors
{
    public class CaretBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<int> CursorPositionProperty = AvaloniaProperty.RegisterAttached<CaretBehavior, Interactive, int>(
            "CursorPosition", default, false, BindingMode.TwoWay, ValidateCursorPosition);

        private static int _index;

        private static int ValidateCursorPosition(Interactive element, int index)
        {
            _index = index;

            var textBox = element as TextBox;
            textBox.AttachedToVisualTree += SetTextBoxCaretIndexAndFocus;
            textBox.PointerReleased += TextBox_PointerReleased;

            SetTextBoxCaretIndexAndFocus(textBox, null);

            return index;
        }

        private static void SetTextBoxCaretIndexAndFocus(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.CaretIndex = _index;
            textBox.Focus();
        }

        private static void TextBox_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var textBox = sender as TextBox;
            SetCursorPosition(textBox, textBox.CaretIndex);
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
