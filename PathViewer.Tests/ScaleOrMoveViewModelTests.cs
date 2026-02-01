using System.Windows;

using PathViewer.Tests.Mocks;

using Xunit;

namespace PathViewer.Tests;

public class ScaleOrMoveViewModelBasicTests
{
    [Fact]
    public void DefaultConstructor_SetsDefaultValues()
    {
        var vm = new ScaleOrMoveViewModel();

        Assert.Equal(100, vm.Width);
        Assert.Equal(100, vm.Height);
        Assert.False(vm.IsScaleMode);
    }

    [Fact]
    public void Constructor_SetsWidthAndHeight()
    {
        var vm = new ScaleOrMoveViewModel(200, 150, false);

        Assert.Equal(200, vm.Width);
        Assert.Equal(150, vm.Height);
    }

    [Fact]
    public void Constructor_SetsIsScaleMode()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, true);

        Assert.True(vm.IsScaleMode);
    }

    [Fact]
    public void IsAspectLocked_DefaultsToTrue()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, true);

        Assert.True(vm.IsAspectLocked);
    }
}

public class ScaleOrMoveViewModelAspectRatioTests
{
    [Fact]
    public void Width_WhenScaleModeAndLocked_UpdatesHeight()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, true);
        vm.IsAspectLocked = true;

        vm.Width = 200;

        Assert.Equal(200, vm.Width);
        Assert.Equal(100, vm.Height); // Height should double (2x scale)
    }

    [Fact]
    public void Height_WhenScaleModeAndLocked_UpdatesWidth()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, true);
        vm.IsAspectLocked = true;

        vm.Height = 100;

        Assert.Equal(100, vm.Height);
        Assert.Equal(200, vm.Width); // Width should double (2x scale)
    }

    [Fact]
    public void Width_WhenScaleModeAndUnlocked_DoesNotUpdateHeight()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, true);
        vm.IsAspectLocked = false;

        vm.Width = 200;

        Assert.Equal(200, vm.Width);
        Assert.Equal(50, vm.Height); // Height should remain unchanged
    }

    [Fact]
    public void Height_WhenScaleModeAndUnlocked_DoesNotUpdateWidth()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, true);
        vm.IsAspectLocked = false;

        vm.Height = 100;

        Assert.Equal(100, vm.Height);
        Assert.Equal(100, vm.Width); // Width should remain unchanged
    }

    [Fact]
    public void Width_WhenMoveMode_DoesNotUpdateHeight()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, false);
        vm.IsAspectLocked = true; // Even with aspect locked

        vm.Width = 200;

        Assert.Equal(200, vm.Width);
        Assert.Equal(50, vm.Height); // Height should remain unchanged in move mode
    }

    [Fact]
    public void Height_WhenMoveMode_DoesNotUpdateWidth()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, false);
        vm.IsAspectLocked = true; // Even with aspect locked

        vm.Height = 100;

        Assert.Equal(100, vm.Height);
        Assert.Equal(100, vm.Width); // Width should remain unchanged in move mode
    }

    [Fact]
    public void AspectRatio_PreservedWithMultipleChanges()
    {
        var vm = new ScaleOrMoveViewModel(100, 200, true);
        vm.IsAspectLocked = true;

        // Initial ratio is 1:2 (width:height)
        vm.Width = 50; // Scale down by 0.5x

        Assert.Equal(50, vm.Width);
        Assert.Equal(100, vm.Height); // Height should also halve

        vm.Height = 200; // Scale up by 2x

        Assert.Equal(200, vm.Height);
        Assert.Equal(100, vm.Width); // Width should also double
    }

    [Fact]
    public void Width_WhenSetToSameValue_DoesNotTriggerHeightUpdate()
    {
        var vm = new ScaleOrMoveViewModel(100, 50, true);
        vm.IsAspectLocked = true;

        vm.Width = 100; // Same value

        Assert.Equal(100, vm.Width);
        Assert.Equal(50, vm.Height); // Height should remain unchanged
    }
}

public class ScaleOrMoveViewModelLabelTests
{
    [Fact]
    public void WidthLabel_InScaleMode_ReturnsPathWidth()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, true);

        Assert.Equal("PathWidth", vm.WidthLabel);
    }

    [Fact]
    public void WidthLabel_InMoveMode_ReturnsX()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, false);

        Assert.Equal("X", vm.WidthLabel);
    }

    [Fact]
    public void HeightLabel_InScaleMode_ReturnsPathHeight()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, true);

        Assert.Equal("PathHeight", vm.HeightLabel);
    }

    [Fact]
    public void HeightLabel_InMoveMode_ReturnsY()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, false);

        Assert.Equal("Y", vm.HeightLabel);
    }
}

public class ScaleOrMoveViewModelOkTests
{
    [Fact]
    public void Ok_InMoveMode_RaisesCloseEvent()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, false);
        bool closeCalled = false;
        vm.CloseEvent += (s, e) => closeCalled = true;

        vm.Ok();

        Assert.True(closeCalled);
    }

    [Fact]
    public void Ok_InMoveMode_WithNegativeValues_StillCloses()
    {
        var vm = new ScaleOrMoveViewModel(-10, -20, false);
        bool closeCalled = false;
        vm.CloseEvent += (s, e) => closeCalled = true;

        vm.Ok();

        Assert.True(closeCalled); // Move mode doesn't validate
    }

    [Fact]
    public void Ok_InMoveMode_WithZeroValues_StillCloses()
    {
        var vm = new ScaleOrMoveViewModel(0, 0, false);
        bool closeCalled = false;
        vm.CloseEvent += (s, e) => closeCalled = true;

        vm.Ok();

        Assert.True(closeCalled); // Move mode doesn't validate
    }

    [Fact]
    public void Ok_InScaleMode_WithPositiveValues_RaisesCloseEvent()
    {
        var vm = new ScaleOrMoveViewModel(100, 100, true);
        bool closeCalled = false;
        vm.CloseEvent += (s, e) => closeCalled = true;

        vm.Ok();

        Assert.True(closeCalled);
    }
}

public class ScaleOrMoveViewModelValidationTests
{
    [Fact]
    public void Ok_InScaleMode_WithZeroWidth_ShowsMessageBox()
    {
        var mockDialog = new MockDialogService();
        var vm = new ScaleOrMoveViewModel(0, 100, true, mockDialog);

        vm.Ok();

        Assert.Equal(1, mockDialog.ShowMessageBoxCallCount);
        Assert.Contains("greater than zero", mockDialog.LastMessageBoxMessage);
    }

    [Fact]
    public void Ok_InScaleMode_WithZeroHeight_ShowsMessageBox()
    {
        var mockDialog = new MockDialogService();
        var vm = new ScaleOrMoveViewModel(100, 0, true, mockDialog);

        vm.Ok();

        Assert.Equal(1, mockDialog.ShowMessageBoxCallCount);
    }

    [Fact]
    public void Ok_InScaleMode_WithNegativeWidth_ShowsMessageBox()
    {
        var mockDialog = new MockDialogService();
        var vm = new ScaleOrMoveViewModel(-10, 100, true, mockDialog);

        vm.Ok();

        Assert.Equal(1, mockDialog.ShowMessageBoxCallCount);
    }

    [Fact]
    public void Ok_InScaleMode_WithNegativeHeight_ShowsMessageBox()
    {
        var mockDialog = new MockDialogService();
        var vm = new ScaleOrMoveViewModel(100, -10, true, mockDialog);

        vm.Ok();

        Assert.Equal(1, mockDialog.ShowMessageBoxCallCount);
    }

    [Fact]
    public void Ok_InScaleMode_WhenUserClicksOk_DoesNotClose()
    {
        var mockDialog = new MockDialogService { MessageBoxResultToReturn = MessageBoxResult.OK };
        var vm = new ScaleOrMoveViewModel(0, 100, true, mockDialog);
        bool closeCalled = false;
        vm.CloseEvent += (s, e) => closeCalled = true;

        vm.Ok();

        Assert.False(closeCalled); // User clicked OK to edit values, dialog stays open
    }

    [Fact]
    public void Ok_InScaleMode_WhenUserClicksCancel_ClosesWithCancel()
    {
        var mockDialog = new MockDialogService { MessageBoxResultToReturn = MessageBoxResult.Cancel };
        var vm = new ScaleOrMoveViewModel(0, 100, true, mockDialog);
        bool closeCalled = false;
        bool cancelValue = false;
        vm.CloseEvent += (s, e) =>
        {
            closeCalled = true;
            cancelValue = e.Cancel;
        };

        vm.Ok();

        Assert.True(closeCalled);
        Assert.True(cancelValue); // Cancel should be true
    }

    [Fact]
    public void Ok_InScaleMode_WithValidValues_DoesNotShowMessageBox()
    {
        var mockDialog = new MockDialogService();
        var vm = new ScaleOrMoveViewModel(100, 100, true, mockDialog);

        vm.Ok();

        Assert.Equal(0, mockDialog.ShowMessageBoxCallCount);
    }
}
