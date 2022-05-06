﻿using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SapphireNotes.ViewModels.UserControls;

namespace SapphireNotes.UserControls;

public class NoteTab : UserControl
{
    public NoteTab()
    {
        InitializeComponent();

        var editor = this.FindControl<TextBox>("editor");
        editor.Initialized += Editor_Initialized;

        DataContextChanged += NoteTab_DataContextChanged;
    }

    private void Editor_Initialized(object sender, EventArgs e)
    {
        var editor = sender as TextBox;
        editor.AddHandler(PointerReleasedEvent, UpdateViewModelCaretPosition);
        editor.AddHandler(KeyUpEvent, UpdateViewModelCaretPosition);

        var vm = DataContext as NoteViewModel;
        editor.CaretIndex = vm.CaretPosition;
        editor.Focus();

        void UpdateViewModelCaretPosition(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NoteViewModel;
            vm.CaretPosition = editor.CaretIndex;
        }
    }

    private void NoteTab_DataContextChanged(object sender, EventArgs e)
    {
        if (DataContext is NoteViewModel vm)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                SetCaretPositionAndFocus(vm);
            });
        }

        void SetCaretPositionAndFocus(NoteViewModel vm)
        {
            var editor = this.FindControl<TextBox>("editor");
            editor.CaretIndex = vm.CaretPosition;
            editor.Focus();
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
