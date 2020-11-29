using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace SapphireNotes.Views
{
    public class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var okButton = this.FindControl<Button>("okButton");
            okButton.Command = ReactiveCommand.Create(OkButtonClicked);
        }

        private void OkButtonClicked()
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
