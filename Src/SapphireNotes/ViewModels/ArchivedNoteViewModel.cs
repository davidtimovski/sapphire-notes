using System;
using System.ComponentModel;
using System.Reactive;
using Avalonia.Media;
using ReactiveUI;
using SapphireNotes.Models;
using SapphireNotes.Utils;

namespace SapphireNotes.ViewModels
{
    public class ArchivedNoteViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ArchivedNoteViewModel(Note note)
        {
            Note = note;
            OnMiddleClickCommand = ReactiveCommand.Create(() => MiddleMouseClicked.Invoke(this, null));

            name = note.Name;
            text = note.Text;
            fontFamily = FontFamilyUtil.FontFamilyFromFont(note.Metadata.FontFamily);
            fontSize = note.Metadata.FontSize;
            archivedDate = DateTimeUtil.GetRelativeDate(note.Metadata.Archived.Value);
        }

        public Note Note { get; set; }

        public event EventHandler<EventArgs> MiddleMouseClicked;

        private string name;
        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private string text;
        private string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
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

        private string archivedDate;
        private string ArchivedDate
        {
            get => archivedDate;
            set => this.RaiseAndSetIfChanged(ref archivedDate, value);
        }

        private ReactiveCommand<Unit, Unit> OnMiddleClickCommand { get; }
    }
}
