using System.Windows;

namespace PathViewer.Services;

public interface IDialogService
{
    bool ShowModal(ViewModelBase viewModel, string title);

    MessageBoxResult ShowMessageBox(
        string message,
        string title,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None);
}
