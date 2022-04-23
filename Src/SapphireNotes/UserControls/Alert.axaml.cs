using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SapphireNotes.UserControls;

public class Alert : UserControl
{
    public Alert()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
