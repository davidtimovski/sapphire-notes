using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.Exceptions;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class EditNoteWindow : Window
    {
        public EditNoteWindow()
        {
            InitializeComponent();

            var saveButton = this.FindControl<Button>("saveButton");
            saveButton.Command = ReactiveCommand.Create(SaveButtonClicked);

            var cancelButton = this.FindControl<Button>("cancelButton");
            cancelButton.Command = ReactiveCommand.Create(CancelButtonClicked);
        }

        private void SaveButtonClicked()
        {
            var vm = (EditNoteViewModel)DataContext;

            try
            {
                if (vm.IsNew)
                {
                    vm.Create();
                }
                else
                {
                    vm.Update();
                }

                Close();
            }
            catch (ValidationException)
            {
                // Do nothing. Handled in view model.
            }
        }

        private void CancelButtonClicked()
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
