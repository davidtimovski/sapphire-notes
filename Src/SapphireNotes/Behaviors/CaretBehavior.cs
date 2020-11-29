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
            textBox.AttachedToVisualTree += TextBox_AttachedToVisualTree;
            textBox.PointerReleased += TextBox_PointerReleased;

            return index;
        }

        private static void TextBox_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var textbox = sender as TextBox;
            SetCursorPosition(textbox, textbox.CaretIndex);
        }

        private static void TextBox_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
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
