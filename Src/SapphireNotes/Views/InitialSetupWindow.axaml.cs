using System;
using System.Threading.Tasks;
using Avalonia;
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

#if DEBUG
            this.AttachDevTools();
#endif

            var selectButton = this.FindControl<Button>("selectButton");
            selectButton.Command = ReactiveCommand.Create(SelectButtonClicked);

            var saveButton = this.FindControl<Button>("saveButton");
            saveButton.Command = ReactiveCommand.Create(SaveButtonClicked);
        }

        public event EventHandler<EventArgs> Saved;

        public async Task SelectButtonClicked()
        {
            var directory = await new OpenFolderDialog().ShowAsync(this);

            var vm = (InitialSetupViewModel)DataContext;
            vm.SetNotesDirectory(directory);
        }

        private void SaveButtonClicked()
        {
            Saved.Invoke(this, null);
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
