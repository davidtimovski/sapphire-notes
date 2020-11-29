using System.ComponentModel;
using ReactiveUI;

namespace SapphireNotes.ViewModels
{
    public class ErrorViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ErrorViewModel(string title, string message)
        {
            this.title = title;
            this.message = message;
        }

        private string title;
        private string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private string message;
        private string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }
    }
}
