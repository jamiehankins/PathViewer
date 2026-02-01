using PathViewer.Services;

namespace PathViewer.Tests.Mocks;

public class MockDialogService : IDialogService
{
    public bool ResultToReturn { get; set; } = true;
    public ViewModelBase? LastViewModel { get; private set; }
    public string? LastTitle { get; private set; }
    public int ShowModalCallCount { get; private set; }

    public bool ShowModal(ViewModelBase viewModel, string title)
    {
        ShowModalCallCount++;
        LastViewModel = viewModel;
        LastTitle = title;
        return ResultToReturn;
    }
}
