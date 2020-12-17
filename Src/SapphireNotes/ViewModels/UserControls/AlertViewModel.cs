using System.ComponentModel;
using ReactiveUI;

namespace SapphireNotes.ViewModels.UserControls
{
    public class AlertViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public AlertViewModel(int maxWidth)
        {
            this.maxWidth = maxWidth;
        }

        public void Show(string message)
        {
            Message = message;
            IsVisible = true;
            Opacity = 1;
        }

        public void Hide()
        {
            if (isVisible)
            {
                Opacity = 0;
                IsVisible = false;
                Message = null;
            }
        }

        private int maxWidth;
        private int MaxWidth
        {
            get => maxWidth;
            set => this.RaiseAndSetIfChanged(ref maxWidth, value);
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
