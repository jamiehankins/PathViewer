namespace PathViewer.Services;

public interface IDialogService
{
    bool ShowModal(ViewModelBase viewModel, string title);
}
