using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views;

public class EditNoteWindow : Window
{
    public EditNoteWindow()
    {
        InitializeComponent();

        var saveButton = this.FindControl<Button>("saveButton");
        saveButton.Command = ReactiveCommand.Create(SaveButtonClicked);

        var cancelButton = this.FindControl<Button>("cancelButton");
        cancelButton.Command = ReactiveCommand.Create(CancelButtonClicked);
    }

    private void SaveButtonClicked()
    {
        var vm = (EditNoteViewModel)DataContext;

        bool success;
        if (vm.IsNew)
        {
            success = vm.Create();
        }
        else
        {
            success = vm.Update();
        }

        if (success)
        {
            Close();
        }
    }

    private void CancelButtonClicked()
    {
        Close();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
