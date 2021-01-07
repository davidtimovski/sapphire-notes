using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace SapphireNotes.Views
{
    public class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

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
