using System;
using System.ComponentModel;
using System.Reactive;
using SapphireNotes.Models;
using ReactiveUI;
using SapphireNotes.Utils;
using Avalonia.Media;

namespace SapphireNotes.ViewModels
{
    public class NoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public NoteViewModel(Note note)
        {
            FilePath = note.FilePath;
            name = note.Name;
            text = note.Text;
            fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
            fontSize = note.Metadata.FontSize;
            caretPosition = note.Metadata.CaretPosition;

            OnEditCommand = ReactiveCommand.Create(Edit);
            OnArchiveCommand = ReactiveCommand.Create(Archive);
            OnDeleteCommand = ReactiveCommand.Create(Delete);
        }

        public event EventHandler<EventArgs> Edited;
        public event EventHandler<EventArgs> Archived;
        public event EventHandler<EventArgs> Deleted;

        public string FilePath { get; set; }

        public void Edit()
        {
            Edited.Invoke(this, null);
        }

        public void Archive()
        {
            Archived.Invoke(this, null);
        }

        public void Delete()
        {
            Deleted.Invoke(this, null);
        }

        public bool IsDirty { get; set; }

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
                IsDirty = true;
            }
        }

        private int fontSize;
        public int FontSize
        {
            get => fontSize;
            set => this.RaiseAndSetIfChanged(ref fontSize, value);
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get => fontFamily;
            set => this.RaiseAndSetIfChanged(ref fontFamily, value);
        }

        private int caretPosition;
        public int CaretPosition
        {
            get => caretPosition;
            set => this.RaiseAndSetIfChanged(ref caretPosition, value);
        }

        public Note ToNote()
        {
            return new Note
            {
                FilePath = FilePath,
                IsDirty = IsDirty,
                Name = Name,
                Text = Text,
                Metadata = new NoteMetadata
                {
                    FontFamily = FontFamily.Name,
                    FontSize = fontSize,
                    CaretPosition = caretPosition
                }
            };
        }

        private ReactiveCommand<Unit, Unit> OnEditCommand { get; }
        private ReactiveCommand<Unit, Unit> OnArchiveCommand { get; }
        private ReactiveCommand<Unit, Unit> OnDeleteCommand { get; }
    }
}
