using System.Windows;

using PathViewer.Services;

namespace PathViewer.Tests.Mocks;

public class MockDialogService : IDialogService
{
    public bool ResultToReturn { get; set; } = true;
    public ViewModelBase? LastViewModel { get; private set; }
    public string? LastTitle { get; private set; }
    public int ShowModalCallCount { get; private set; }

    public MessageBoxResult MessageBoxResultToReturn { get; set; } = MessageBoxResult.OK;
    public string? LastMessageBoxMessage { get; private set; }
    public string? LastMessageBoxTitle { get; private set; }
    public int ShowMessageBoxCallCount { get; private set; }

    public bool ShowModal(ViewModelBase viewModel, string title)
    {
        ShowModalCallCount++;
        LastViewModel = viewModel;
        LastTitle = title;
        return ResultToReturn;
    }

    public MessageBoxResult ShowMessageBox(
        string message,
        string title,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None)
    {
        ShowMessageBoxCallCount++;
        LastMessageBoxMessage = message;
        LastMessageBoxTitle = title;
        return MessageBoxResultToReturn;
    }
}
