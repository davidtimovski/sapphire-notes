using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels
{
    public class NoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public NoteViewModel(Note note)
        {
            Note = note;

            name = note.Name;
            text = note.Text;
            fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
            fontSize = note.Metadata.FontSize;
            caretPosition = note.Metadata.CaretPosition;

            OnEditCommand = ReactiveCommand.Create(() => EditClicked.Invoke(this, null));
            OnArchiveCommand = ReactiveCommand.Create(() => ArchiveClicked.Invoke(this, null));
            OnDeleteCommand = ReactiveCommand.Create(() => DeleteClicked.Invoke(this, null));
        }

        public Note Note { get; set; }

        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> ArchiveClicked;
        public event EventHandler<EventArgs> DeleteClicked;

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref text, value);
                Note.IsDirty = true;
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get => fontFamily;
            set => this.RaiseAndSetIfChanged(ref fontFamily, value);
        }

        private int fontSize;
        public int FontSize
        {
            get => fontSize;
            set => this.RaiseAndSetIfChanged(ref fontSize, value);
        }

        private int caretPosition;
        public int CaretPosition
        {
            get => caretPosition;
            set => this.RaiseAndSetIfChanged(ref caretPosition, value);
        }

        public Note ToNote()
        {
            Note.Name = name;
            Note.Text = text;
            Note.Metadata.FontFamily = fontFamily.Name;
            Note.Metadata.FontSize = fontSize;
            Note.Metadata.CaretPosition = caretPosition;

            return Note;
        }

        private ReactiveCommand<Unit, Unit> OnEditCommand { get; }
        private ReactiveCommand<Unit, Unit> OnArchiveCommand { get; }
        private ReactiveCommand<Unit, Unit> OnDeleteCommand { get; }
    }
}
