using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Behaviors
{
    public class ClickBehavior : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> MiddleMouseArchiveProperty = AvaloniaProperty.RegisterAttached<ClickBehavior, Interactive, bool>(
            "MiddleMouseArchive", default, false, BindingMode.OneTime, ValidateMiddleMouseArchive);

        private static bool ValidateMiddleMouseArchive(Interactive element, bool value)
        {
            var tabItemHeader = element as Border;
            tabItemHeader.PointerReleased += Item_PointerReleased;

            return value;

            void Item_PointerReleased(object s, PointerReleasedEventArgs e)
            {
                if (e.GetCurrentPoint(tabItemHeader).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased)
                {
                    var vm = tabItemHeader.DataContext as NoteViewModel;
                    vm.InvokeMiddleMouseClick();
                }
            }
        }

        public static void SetMiddleMouseArchive(AvaloniaObject element, bool value)
        {
            element.SetValue(MiddleMouseArchiveProperty, value);
        }

        public static bool GetMiddleMouseArchive(AvaloniaObject element)
        {
            return element.GetValue(MiddleMouseArchiveProperty);
        }
    }
}
