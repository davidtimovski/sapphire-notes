using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views
{
    public class PreferencesWindow : Window
    {
        public PreferencesWindow()
        {
            InitializeComponent();

            var changeNotesDirectoryButton = this.FindControl<Button>("changeNotesDirectoryButton");
            changeNotesDirectoryButton.Command = ReactiveCommand.Create(ChangeNotesDirectoryButtonClicked);

            var applyButton = this.FindControl<Button>("applyButton");
            applyButton.Command = ReactiveCommand.Create(ApplyButtonClicked);

            var closeButton = this.FindControl<Button>("closeButton");
            closeButton.Command = ReactiveCommand.Create(CloseButtonClicked);
        }

        public async Task ChangeNotesDirectoryButtonClicked()
        {
            var directory = await new OpenFolderDialog().ShowAsync(this);

            var vm = (PreferencesViewModel)DataContext;
            vm.SetNotesDirectory(directory);
        }

        private void ApplyButtonClicked()
        {
            var vm = (PreferencesViewModel)DataContext;
            vm.Save();

            Close();
        }

        private void CloseButtonClicked()
        {
            Close();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
