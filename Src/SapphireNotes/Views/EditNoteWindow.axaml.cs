using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class EditNoteWindow : Window
    {
        public EditNoteWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var saveButton = this.FindControl<Button>("saveButton");
            saveButton.Command = ReactiveCommand.Create(SaveButtonClicked);

            var cancelButton = this.FindControl<Button>("cancelButton");
            cancelButton.Command = ReactiveCommand.Create(CancelButtonClicked);
        }

        public event EventHandler<CreatedNoteEventArgs> Created;

        private void SaveButtonClicked()
        {
            var vm = (EditNoteViewModel)DataContext;

            try
            {
                if (vm.IsNew)
                {
                    NoteViewModel note = vm.Create();
                    Created.Invoke(this, new CreatedNoteEventArgs
                    {
                        Note = note
                    });
                }
                else
                {
                    vm.Update();
                }

                Close();
            }
            catch
            {
                var noteNameTextBox = this.FindControl<TextBox>("noteNameTextBox");
                noteNameTextBox.BorderBrush = this.FindResource("DangerColor") as SolidColorBrush;
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

    public class CreatedNoteEventArgs : EventArgs
    {
        public NoteViewModel Note { get; set; }
    }
}
