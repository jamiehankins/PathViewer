using Xunit;

namespace PathViewer.Tests;

public class PathViewModelTests
{
    [Fact]
    public void NewViewModel_HasDefaultData()
    {
        var viewModel = new PathViewModel();

        Assert.False(string.IsNullOrEmpty(viewModel.Data));
        Assert.StartsWith("M", viewModel.Data);
    }
}
