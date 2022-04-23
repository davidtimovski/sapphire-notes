using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using SapphireNotes.ViewModels;

namespace SapphireNotes.Views;

public class QuickNoteWindow : Window
{
    public QuickNoteWindow()
    {
        InitializeComponent();

        var createButton = this.FindControl<Button>("createButton");
        createButton.Command = ReactiveCommand.Create(CreateButtonClicked);

        var cancelButton = this.FindControl<Button>("cancelButton");
        cancelButton.Command = ReactiveCommand.Create(CancelButtonClicked);
    }

    private void CreateButtonClicked()
    {
        var vm = (QuickNoteViewModel)DataContext;
        vm.Create();

        Close();
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
