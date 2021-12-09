using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class InitialSetupWindow : Window
    {
        public InitialSetupWindow()
        {
            InitializeComponent();

            var selectButton = this.FindControl<Button>("selectButton");
            selectButton.Command = ReactiveCommand.Create(SelectButtonClicked);

            var startButton = this.FindControl<Button>("startButton");
            startButton.Command = ReactiveCommand.Create(StartButtonClicked);
        }

        public event EventHandler<EventArgs> Started;

        public async Task SelectButtonClicked()
        {
            var directory = await new OpenFolderDialog().ShowAsync(this);

            var vm = (InitialSetupViewModel)DataContext;
            vm.SelectNotesDirectory(directory);
        }

        private void StartButtonClicked()
        {
            var vm = (InitialSetupViewModel)DataContext;
            vm.SaveNotesDirectory();

            Started.Invoke(this, null);
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
