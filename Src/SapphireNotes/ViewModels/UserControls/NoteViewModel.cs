using System;
using System.Reactive;
using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Contracts.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels.UserControls
{
    public class NoteViewModel : ViewModelBase
    {
        private readonly Note _model;

        public NoteViewModel(Note note)
        {
            _model = note;

            OnEditCommand = ReactiveCommand.Create(() => EditClicked.Invoke(this, null));
            OnArchiveCommand = ReactiveCommand.Create(() => ArchiveClicked.Invoke(this, null));
            OnDeleteCommand = ReactiveCommand.Create(() => DeleteClicked.Invoke(this, null));
            OnMiddleClickCommand = ReactiveCommand.Create(() => MiddleMouseClicked.Invoke(this, null));

            name = note.Name;
            content = note.Content;
            fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
            fontSize = note.Metadata.FontSize;
            caretPosition = note.Metadata.CaretPosition;

            SetShortenedName();
        }

        public event EventHandler<EventArgs> EditClicked;
        public event EventHandler<EventArgs> ArchiveClicked;
        public event EventHandler<EventArgs> DeleteClicked;
        public event EventHandler<EventArgs> MiddleMouseClicked;

        public bool IsDirty { get; private set; }

        public void SetPristine()
        {
            IsDirty = false;
        }

        private string name;
        public string Name
        {
            get => name;
            set 
            {
                this.RaiseAndSetIfChanged(ref name, value);
                SetShortenedName();
            }
        }

        private string shortenedName;
        public string ShortenedName
        {
            get => shortenedName;
            set => this.RaiseAndSetIfChanged(ref shortenedName, value);
        }

        private string content;
        public string Content
        {
            get => content;
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
                IsDirty = true;
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
            _model.Name = name;
            _model.Content = content;
            _model.Metadata.CaretPosition = caretPosition;
            _model.IsDirty = IsDirty;

            return _model;
        }

        private ReactiveCommand<Unit, Unit> OnEditCommand { get; }
        private ReactiveCommand<Unit, Unit> OnArchiveCommand { get; }
        private ReactiveCommand<Unit, Unit> OnDeleteCommand { get; }
        private ReactiveCommand<Unit, Unit> OnMiddleClickCommand { get; }

        private void SetShortenedName()
        {
            ShortenedName = name.Length > 35 ? $"{name[..33]}.." : name;
        }
    }
}
