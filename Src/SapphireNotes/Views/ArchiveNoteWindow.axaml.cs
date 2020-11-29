using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SapphireNotes.Models;
using SapphireNotes.ViewModels;
using ReactiveUI;

namespace SapphireNotes.Views
{
    public class ArchiveNoteWindow : Window
    {
        public ArchiveNoteWindow()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif

            var yesButton = this.FindControl<Button>("yesButton");
            yesButton.Command = ReactiveCommand.Create(YesButtonClicked);

            var noButton = this.FindControl<Button>("noButton");
            noButton.Command = ReactiveCommand.Create(NoButtonClicked);
        }

        public event EventHandler<ArchivedNoteEventArgs> Archived;

        private void YesButtonClicked()
        {
            var vm = (ArchiveNoteViewModel)DataContext;

            NoteViewModel note = vm.Archive();
            Archived.Invoke(this, new ArchivedNoteEventArgs
            {
                Note = note
            });

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

    public class ArchivedNoteEventArgs : EventArgs
    {
        public NoteViewModel Note { get; set; }
    }
}
