using PathViewer.Tests.Mocks;
using Xunit;

namespace PathViewer.Tests;

public class PathViewModelTests
{
    private static PathViewModel CreateViewModel()
    {
        return new PathViewModel(
            new MockDialogService(),
            new MockPreferencesService());
    }

    [Fact]
    public void NewViewModel_HasDefaultData()
    {
        var viewModel = CreateViewModel();

        Assert.False(string.IsNullOrEmpty(viewModel.Data));
        Assert.StartsWith("M", viewModel.Data);
    }

    [Fact]
    public void NewViewModel_HasPathCommands()
    {
        var viewModel = CreateViewModel();

        Assert.NotEmpty(viewModel.PathCommands);
    }

    [Fact]
    public void NewViewModel_SelectedIndexIsMinusOne()
    {
        var viewModel = CreateViewModel();

        Assert.Equal(-1, viewModel.SelectedIndex);
    }
}

public class CanExecutePredicateTests
{
    private static PathViewModel CreateViewModel()
    {
        return new PathViewModel(
            new MockDialogService(),
            new MockPreferencesService());
    }

    [Fact]
    public void DeleteCommand_CannotExecute_WhenNothingSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = -1;

        Assert.False(viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void DeleteCommand_CanExecute_WhenItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        Assert.True(viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ClearCommand_CanExecute_WhenHasCommands()
    {
        var viewModel = CreateViewModel();

        Assert.True(viewModel.ClearCommand.CanExecute(null));
    }

    [Fact]
    public void ClearCommand_CannotExecute_WhenNoCommands()
    {
        var viewModel = CreateViewModel();
        viewModel.ClearCommand.Execute(null);

        Assert.False(viewModel.ClearCommand.CanExecute(null));
    }

    [Fact]
    public void MoveUpCommand_CannotExecute_WhenNothingSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = -1;

        Assert.False(viewModel.MoveUpCommand.CanExecute(null));
    }

    [Fact]
    public void MoveUpCommand_CannotExecute_WhenFirstItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        Assert.False(viewModel.MoveUpCommand.CanExecute(null));
    }

    [Fact]
    public void MoveUpCommand_CanExecute_WhenSecondItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 1;

        Assert.True(viewModel.MoveUpCommand.CanExecute(null));
    }

    [Fact]
    public void MoveDownCommand_CannotExecute_WhenNothingSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = -1;

        Assert.False(viewModel.MoveDownCommand.CanExecute(null));
    }

    [Fact]
    public void MoveDownCommand_CannotExecute_WhenLastItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = viewModel.PathCommands.Count - 1;

        Assert.False(viewModel.MoveDownCommand.CanExecute(null));
    }

    [Fact]
    public void MoveDownCommand_CanExecute_WhenNotLastItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        Assert.True(viewModel.MoveDownCommand.CanExecute(null));
    }

    [Fact]
    public void EditCommand_CannotExecute_WhenNothingSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = -1;

        Assert.False(viewModel.EditCommand.CanExecute(null));
    }

    [Fact]
    public void EditCommand_CanExecute_WhenItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        Assert.True(viewModel.EditCommand.CanExecute(null));
    }

    [Fact]
    public void InsertItemCommand_CannotExecute_WhenNothingSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = -1;

        Assert.False(viewModel.InsertItemCommand.CanExecute(null));
    }

    [Fact]
    public void InsertItemCommand_CanExecute_WhenItemSelected()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        Assert.True(viewModel.InsertItemCommand.CanExecute(null));
    }
}

public class DeleteCommandTests
{
    private static PathViewModel CreateViewModel()
    {
        return new PathViewModel(
            new MockDialogService(),
            new MockPreferencesService());
    }

    [Fact]
    public void Delete_RemovesSelectedItem()
    {
        var viewModel = CreateViewModel();
        int initialCount = viewModel.PathCommands.Count;
        viewModel.SelectedIndex = 0;
        var itemToDelete = viewModel.PathCommands[0];

        viewModel.DeleteCommand.Execute(null);

        Assert.Equal(initialCount - 1, viewModel.PathCommands.Count);
        Assert.DoesNotContain(itemToDelete, viewModel.PathCommands);
    }

    [Fact]
    public void Delete_UpdatesData()
    {
        var viewModel = CreateViewModel();
        string originalData = viewModel.Data;
        viewModel.SelectedIndex = 0;

        viewModel.DeleteCommand.Execute(null);

        Assert.NotEqual(originalData, viewModel.Data);
    }

    [Fact]
    public void Delete_MaintainsSelectionWhenPossible()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 1;

        viewModel.DeleteCommand.Execute(null);

        Assert.True(viewModel.SelectedIndex >= 0);
    }
}

public class ClearCommandTests
{
    private static PathViewModel CreateViewModel()
    {
        return new PathViewModel(
            new MockDialogService(),
            new MockPreferencesService());
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var viewModel = CreateViewModel();
        Assert.NotEmpty(viewModel.PathCommands);

        viewModel.ClearCommand.Execute(null);

        Assert.Empty(viewModel.PathCommands);
    }

    [Fact]
    public void Clear_ClearsData()
    {
        var viewModel = CreateViewModel();

        viewModel.ClearCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.Data);
    }
}

public class MoveCommandTests
{
    private static PathViewModel CreateViewModel()
    {
        return new PathViewModel(
            new MockDialogService(),
            new MockPreferencesService());
    }

    [Fact]
    public void MoveUp_SwapsItemWithPrevious()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 1;
        var itemToMove = viewModel.PathCommands[1];
        var itemAbove = viewModel.PathCommands[0];

        viewModel.MoveUpCommand.Execute(null);

        Assert.Equal(itemToMove, viewModel.PathCommands[0]);
        Assert.Equal(itemAbove, viewModel.PathCommands[1]);
    }

    [Fact]
    public void MoveUp_UpdatesSelectedIndex()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 1;

        viewModel.MoveUpCommand.Execute(null);

        Assert.Equal(0, viewModel.SelectedIndex);
    }

    [Fact]
    public void MoveUp_UpdatesData()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 1;
        string originalData = viewModel.Data;

        viewModel.MoveUpCommand.Execute(null);

        Assert.NotEqual(originalData, viewModel.Data);
    }

    [Fact]
    public void MoveDown_SwapsItemWithNext()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;
        var itemToMove = viewModel.PathCommands[0];
        var itemBelow = viewModel.PathCommands[1];

        viewModel.MoveDownCommand.Execute(null);

        Assert.Equal(itemToMove, viewModel.PathCommands[1]);
        Assert.Equal(itemBelow, viewModel.PathCommands[0]);
    }

    [Fact]
    public void MoveDown_UpdatesSelectedIndex()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;

        viewModel.MoveDownCommand.Execute(null);

        Assert.Equal(1, viewModel.SelectedIndex);
    }

    [Fact]
    public void MoveDown_UpdatesData()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedIndex = 0;
        string originalData = viewModel.Data;

        viewModel.MoveDownCommand.Execute(null);

        Assert.NotEqual(originalData, viewModel.Data);
    }
}

public class DialogCommandTests
{
    [Fact]
    public void AddItem_ShowsDialog()
    {
        var mockDialog = new MockDialogService();
        mockDialog.ResultToReturn = false; // User cancels
        var viewModel = new PathViewModel(mockDialog, new MockPreferencesService());

        viewModel.AddItemCommand.Execute(null);

        Assert.Equal(1, mockDialog.ShowModalCallCount);
        Assert.NotNull(mockDialog.LastViewModel);
        Assert.IsType<AddOrEditViewModel>(mockDialog.LastViewModel);
    }

    [Fact]
    public void EditCommand_ShowsDialog_WhenItemSelected()
    {
        var mockDialog = new MockDialogService();
        mockDialog.ResultToReturn = false; // User cancels
        var viewModel = new PathViewModel(mockDialog, new MockPreferencesService());
        viewModel.SelectedIndex = 0;

        viewModel.EditCommand.Execute(null);

        Assert.Equal(1, mockDialog.ShowModalCallCount);
        Assert.NotNull(mockDialog.LastViewModel);
        Assert.IsType<AddOrEditViewModel>(mockDialog.LastViewModel);
    }

    [Fact]
    public void ScalePathCommand_ShowsDialog()
    {
        var mockDialog = new MockDialogService();
        mockDialog.ResultToReturn = false; // User cancels
        var viewModel = new PathViewModel(mockDialog, new MockPreferencesService());

        viewModel.ScalePathCommand.Execute(null);

        Assert.Equal(1, mockDialog.ShowModalCallCount);
        Assert.NotNull(mockDialog.LastViewModel);
        Assert.IsType<ScaleOrMoveViewModel>(mockDialog.LastViewModel);
    }

    [Fact]
    public void MovePathCommand_ShowsDialog()
    {
        var mockDialog = new MockDialogService();
        mockDialog.ResultToReturn = false; // User cancels
        var viewModel = new PathViewModel(mockDialog, new MockPreferencesService());

        viewModel.MovePathCommand.Execute(null);

        Assert.Equal(1, mockDialog.ShowModalCallCount);
        Assert.NotNull(mockDialog.LastViewModel);
        Assert.IsType<ScaleOrMoveViewModel>(mockDialog.LastViewModel);
    }

    [Fact]
    public void InsertItemCommand_ShowsDialog_WhenItemSelected()
    {
        var mockDialog = new MockDialogService();
        mockDialog.ResultToReturn = false; // User cancels
        var viewModel = new PathViewModel(mockDialog, new MockPreferencesService());
        viewModel.SelectedIndex = 0;

        viewModel.InsertItemCommand.Execute(null);

        Assert.Equal(1, mockDialog.ShowModalCallCount);
    }
}

public class PreferencesTests
{
    [Fact]
    public void Constructor_LoadsPreferences()
    {
        var mockPrefs = new MockPreferencesService();

        var viewModel = new PathViewModel(new MockDialogService(), mockPrefs);

        Assert.Equal(1, mockPrefs.LoadCallCount);
    }

    [Fact]
    public void ChangingTheme_SavesPreferences()
    {
        var mockPrefs = new MockPreferencesService();
        var viewModel = new PathViewModel(new MockDialogService(), mockPrefs);
        mockPrefs.SaveCallCount = 0; // Reset after constructor

        viewModel.Theme = "Dark";

        Assert.Equal(1, mockPrefs.SaveCallCount);
    }

    [Fact]
    public void ChangingStrokeColor_SavesPreferences()
    {
        var mockPrefs = new MockPreferencesService();
        var viewModel = new PathViewModel(new MockDialogService(), mockPrefs);
        mockPrefs.SaveCallCount = 0;

        viewModel.StrokeColor = "Red";

        Assert.Equal(1, mockPrefs.SaveCallCount);
    }
}
