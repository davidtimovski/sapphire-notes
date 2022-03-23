using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.Utils;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class ArchivedNotesWindow : Window
    {
        private readonly DebounceDispatcher _debounceTimer = new();

        public ArchivedNotesWindow()
        {
            InitializeComponent();

            var searchTextBox = this.FindControl<TextBox>("searchTextBox");
            searchTextBox.KeyUp += SearchTextBox_KeyUp;

            var closeButton = this.FindControl<Button>("closeButton");
            closeButton.Command = ReactiveCommand.Create(CloseButtonClicked);

            Closing += ArchivedNotesWindow_Closing;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            _debounceTimer.Debounce(100, _ =>
            {
                var vm = (ArchivedNotesViewModel)DataContext;
                vm.SearchTextChanged();
            });
        }

        private void ArchivedNotesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (ArchivedNotesViewModel)DataContext;
            vm.Dispose();
        }

        private void CloseButtonClicked()
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
