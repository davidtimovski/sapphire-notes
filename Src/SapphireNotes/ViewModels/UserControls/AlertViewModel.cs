using ReactiveUI;

namespace SapphireNotes.ViewModels.UserControls
{
    public class AlertViewModel : ViewModelBase
    {
        public AlertViewModel(int maxWidth)
        {
            this.maxWidth = maxWidth;
        }

        public void Show(string message)
        {
            Message = message;
            IsVisible = true;
        }

        public void Hide()
        {
            if (isVisible)
            {
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
    }
}
