using System.ComponentModel;
using ReactiveUI;

namespace SapphireNotes.ViewModels.UserControls
{
    public class AlertViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public AlertViewModel(int width)
        {
            this.width = width;
        }

        public void Show(string message)
        {
            Message = message;
            IsVisible = true;
            Opacity = 1;
        }

        private int width;
        private int Width
        {
            get => width;
            set => this.RaiseAndSetIfChanged(ref width, value);
        }

        private string message;
        private string Message
        {
            get => message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        private bool isVisible;
        private bool IsVisible
        {
            get => isVisible;
            set => this.RaiseAndSetIfChanged(ref isVisible, value);
        }

        private double opacity;
        private double Opacity
        {
            get => opacity;
            set => this.RaiseAndSetIfChanged(ref opacity, value);
        }
    }
}
