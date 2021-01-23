using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace SapphireNotes.Views
{
    public class ArchivedNotesWindow : Window
    {
        public ArchivedNotesWindow()
        {
            InitializeComponent();

            var closeButton = this.FindControl<Button>("closeButton");
            closeButton.Command = ReactiveCommand.Create(CloseButtonClicked);
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
