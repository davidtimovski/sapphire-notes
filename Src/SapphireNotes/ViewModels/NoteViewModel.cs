using System;
using System.ComponentModel;
using System.Reactive;
using SapphireNotes.Models;
using ReactiveUI;

namespace SapphireNotes.ViewModels
{
    public class NoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public NoteViewModel()
        {
            name = string.Empty;
        }

        public NoteViewModel(string name)
        {
            this.name = name;
        }

        public NoteViewModel(string name, string filePath, string fileContents, NoteMetadata metadata)
        {
            FilePath = filePath;
            this.name = name;
            text = fileContents;
            fontSize = metadata.FontSize;
            FontFamily = metadata.FontFamily;
            cursorPosition = metadata.CursorPosition;

            OnEditCommand = ReactiveCommand.Create(Edit);
            OnArchiveCommand = ReactiveCommand.Create(Archive);
            OnDeleteCommand = ReactiveCommand.Create(Delete);
        }

        public NoteViewModel(Note note)
        {
            FilePath = note.FilePath;
            name = note.Name;
            text = note.Text;
            fontSize = note.Metadata.FontSize;
            FontFamily = note.Metadata.FontFamily;
            cursorPosition = note.Metadata.CursorPosition;

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

        public bool IsDirty { get; private set; }

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

        private string fontFamily;
        public string FontFamily
        {
            get => fontFamily;
            set => this.RaiseAndSetIfChanged(ref fontFamily, value);
        }

        private int cursorPosition;
        public int CursorPosition
        {
            get => cursorPosition;
            set => this.RaiseAndSetIfChanged(ref cursorPosition, value);
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
                    FontFamily = fontFamily,
                    FontSize = fontSize,
                    CursorPosition = cursorPosition
                }
            };
        }

        private ReactiveCommand<Unit, Unit> OnEditCommand { get; }
        private ReactiveCommand<Unit, Unit> OnArchiveCommand { get; }
        private ReactiveCommand<Unit, Unit> OnDeleteCommand { get; }
    }
}
