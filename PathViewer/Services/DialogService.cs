using System;
using System.Windows;
using System.Windows.Input;

namespace PathViewer.Services;

public class DialogService : IDialogService
{
    public bool ShowModal(ViewModelBase viewModel, string title)
    {
        bool result;
        Window window = new()
        {
            Content = viewModel,
            Title = title,
            SizeToContent = SizeToContent.WidthAndHeight,
            Owner = Application.Current.MainWindow,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Resources = new ResourceDictionary
            {
                MergedDictionaries =
                {
                    new ResourceDictionary { Source = new Uri("Resources/ControlStyles.xaml", UriKind.Relative) }
                }
            }
        };

        try
        {
            window.KeyUp += OnKey;
            result = window.ShowDialog() ?? false;
        }
        finally
        {
            window.KeyUp -= OnKey;
        }

        return result;

        void OnKey(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                window.DialogResult = false;
                window.Close();
            }
        }
    }

    public MessageBoxResult ShowMessageBox(
        string message,
        string title,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        return MessageBox.Show(message, title, buttons, icon);
    }
}
