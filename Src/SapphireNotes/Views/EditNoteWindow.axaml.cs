using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Models;
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
        public event EventHandler<UpdatedNoteEventArgs> Updated;

        private void SaveButtonClicked()
        {
            var vm = (EditNoteViewModel)DataContext;

            try
            {
                if (vm.IsNew)
                {
                    Note note = vm.Create();
                    Created.Invoke(this, new CreatedNoteEventArgs
                    {
                        CreatedNote = note
                    });
                }
                else
                {
                    var (originalName, updatedNote) = vm.Update();
                    Updated.Invoke(this, new UpdatedNoteEventArgs
                    {
                        OriginalName = originalName,
                        UpdatedNote = updatedNote
                    });
                }

                Close();
            }
            catch
            {
                var noteNameTextBox = this.FindControl<TextBox>("noteNameTextBox");
                noteNameTextBox.BorderBrush = this.FindResource("InvalidBorderColor") as SolidColorBrush;
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
        public Note CreatedNote { get; set; }
    }

    public class UpdatedNoteEventArgs : EventArgs
    {
        public string OriginalName { get; set; }
        public Note UpdatedNote { get; set; }
    }
}
