using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class DeleteNoteWindow : Window
    {
        public DeleteNoteWindow()
        {
            InitializeComponent();

            var yesButton = this.FindControl<Button>("yesButton");
            yesButton.Command = ReactiveCommand.Create(YesButtonClicked);

            var noButton = this.FindControl<Button>("noButton");
            noButton.Command = ReactiveCommand.Create(NoButtonClicked);
        }

        private void YesButtonClicked()
        {
            var vm = (DeleteNoteViewModel)DataContext;
            vm.Delete();

            Close();
        }

        private void NoButtonClicked()
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
