using PathViewer.PathCommands;
using PathViewer.Tests.Mocks;
using Xunit;

namespace PathViewer.Tests;

public class AddOrEditViewModelBasicTests
{
    [Fact]
    public void NewViewModel_DefaultsToMoveType()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());

        Assert.Equal(ItemType.Move, vm.Type.Type);
    }

    [Fact]
    public void NewViewModel_HasSixValues()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());

        Assert.Equal(6, vm.Values.Count);
    }

    [Fact]
    public void NewViewModel_HasThreeFlags()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());

        Assert.Equal(3, vm.Flags.Count);
    }

    [Fact]
    public void NewViewModel_HasTenItemTypes()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());

        Assert.Equal(10, vm.ItemTypes.Length);
    }
}

public class AddOrEditCommandCreationTests
{
    private static AddOrEditViewModel CreateViewModel()
    {
        return new AddOrEditViewModel(new MockDialogService());
    }

    [Fact]
    public void Command_CreatesClose()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Close);

        var command = vm.Command;

        Assert.IsType<Close>(command);
    }

    [Fact]
    public void Command_CreatesMove()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Move);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<Move>(command);
        var move = (Move)command;
        Assert.Equal(10, move.X);
        Assert.Equal(20, move.Y);
        Assert.True(move.IsAbsolute);
    }

    [Fact]
    public void Command_CreatesLine()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Line);
        vm.Values[0].Value = 100;
        vm.Values[1].Value = 200;
        vm.Flags[0].Value = false;

        var command = vm.Command;

        Assert.IsType<Line>(command);
        var line = (Line)command;
        Assert.Equal(100, line.EndX);
        Assert.Equal(200, line.EndY);
        Assert.False(line.IsAbsolute);
    }

    [Fact]
    public void Command_CreatesHorizontalLine()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.HorizontalLine);
        vm.Values[0].Value = 50;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<HorizontalLine>(command);
        var hline = (HorizontalLine)command;
        Assert.Equal(50, hline.EndX);
        Assert.True(hline.IsAbsolute);
    }

    [Fact]
    public void Command_CreatesVerticalLine()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.VerticalLine);
        vm.Values[0].Value = 75;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<VerticalLine>(command);
        var vline = (VerticalLine)command;
        Assert.Equal(75, vline.EndY);
        Assert.True(vline.IsAbsolute);
    }

    [Fact]
    public void Command_CreatesCubicBezier()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.CubicBezier);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Values[2].Value = 30;
        vm.Values[3].Value = 40;
        vm.Values[4].Value = 50;
        vm.Values[5].Value = 60;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<CubicBezier>(command);
        var cubic = (CubicBezier)command;
        Assert.Equal(10, cubic.Control1X);
        Assert.Equal(20, cubic.Control1Y);
        Assert.Equal(30, cubic.Control2X);
        Assert.Equal(40, cubic.Control2Y);
        Assert.Equal(50, cubic.EndX);
        Assert.Equal(60, cubic.EndY);
        Assert.True(cubic.IsAbsolute);
    }

    [Fact]
    public void Command_CreatesQuadraticBezier()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.QuadraticBezier);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Values[2].Value = 30;
        vm.Values[3].Value = 40;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<QuadraticBezier>(command);
        var quad = (QuadraticBezier)command;
        Assert.Equal(10, quad.ControlX);
        Assert.Equal(20, quad.ControlY);
        Assert.Equal(30, quad.EndX);
        Assert.Equal(40, quad.EndY);
    }

    [Fact]
    public void Command_CreatesSmoothCubicBezier()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.SmoothCubicBezier);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Values[2].Value = 30;
        vm.Values[3].Value = 40;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<SmoothCubicBezier>(command);
        var scubic = (SmoothCubicBezier)command;
        Assert.Equal(10, scubic.Control2X);
        Assert.Equal(20, scubic.Control2Y);
        Assert.Equal(30, scubic.EndX);
        Assert.Equal(40, scubic.EndY);
    }

    [Fact]
    public void Command_CreatesSmoothQuadraticBezier()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.SmoothQuadraticBezier);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Flags[0].Value = true;

        var command = vm.Command;

        Assert.IsType<SmoothQuadraticBezier>(command);
        var squad = (SmoothQuadraticBezier)command;
        Assert.Equal(10, squad.EndX);
        Assert.Equal(20, squad.EndY);
    }

    [Fact]
    public void Command_CreatesEllipticalArc()
    {
        var vm = CreateViewModel();
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.EllipticalArc);
        vm.Values[0].Value = 10;  // SizeX
        vm.Values[1].Value = 20;  // SizeY
        vm.Values[2].Value = 45;  // RotationAngle
        vm.Values[3].Value = 100; // EndX
        vm.Values[4].Value = 200; // EndY
        vm.Flags[0].Value = true; // IsAbsolute
        vm.Flags[1].Value = true; // IsLargeArc
        vm.Flags[2].Value = false; // IsPositiveSweepDirection

        var command = vm.Command;

        Assert.IsType<EllipticalArc>(command);
        var arc = (EllipticalArc)command;
        Assert.Equal(10, arc.SizeX);
        Assert.Equal(20, arc.SizeY);
        Assert.Equal(45, arc.RotationAngle);
        Assert.Equal(100, arc.EndX);
        Assert.Equal(200, arc.EndY);
        Assert.True(arc.IsAbsolute);
        Assert.True(arc.IsLargeArc);
        Assert.False(arc.IsPositiveSweepDirection);
    }
}

public class AddOrEditEditModeTests
{
    [Fact]
    public void Constructor_WithMove_PopulatesValues()
    {
        var move = new Move { X = 15, Y = 25, IsAbsolute = true };

        var vm = new AddOrEditViewModel(move, new MockDialogService());

        Assert.Equal(ItemType.Move, vm.Type.Type);
        Assert.Equal(15, vm.Values[0].Value);
        Assert.Equal(25, vm.Values[1].Value);
        Assert.True(vm.Flags[0].Value);
    }

    [Fact]
    public void Constructor_WithLine_PopulatesValues()
    {
        var line = new Line { EndX = 100, EndY = 200, IsAbsolute = false };

        var vm = new AddOrEditViewModel(line, new MockDialogService());

        Assert.Equal(ItemType.Line, vm.Type.Type);
        Assert.Equal(100, vm.Values[0].Value);
        Assert.Equal(200, vm.Values[1].Value);
        Assert.False(vm.Flags[0].Value);
    }

    [Fact]
    public void Constructor_WithHorizontalLine_PopulatesValues()
    {
        var hline = new HorizontalLine { EndX = 50, IsAbsolute = true };

        var vm = new AddOrEditViewModel(hline, new MockDialogService());

        Assert.Equal(ItemType.HorizontalLine, vm.Type.Type);
        Assert.Equal(50, vm.Values[0].Value);
        Assert.True(vm.Flags[0].Value);
    }

    [Fact]
    public void Constructor_WithVerticalLine_PopulatesValues()
    {
        var vline = new VerticalLine { EndY = 75, IsAbsolute = true };

        var vm = new AddOrEditViewModel(vline, new MockDialogService());

        Assert.Equal(ItemType.VerticalLine, vm.Type.Type);
        Assert.Equal(75, vm.Values[0].Value);
        Assert.True(vm.Flags[0].Value);
    }

    [Fact]
    public void Constructor_WithCubicBezier_PopulatesValues()
    {
        var cubic = new CubicBezier
        {
            Control1X = 10,
            Control1Y = 20,
            Control2X = 30,
            Control2Y = 40,
            EndX = 50,
            EndY = 60,
            IsAbsolute = true
        };

        var vm = new AddOrEditViewModel(cubic, new MockDialogService());

        Assert.Equal(ItemType.CubicBezier, vm.Type.Type);
        Assert.Equal(10, vm.Values[0].Value);
        Assert.Equal(20, vm.Values[1].Value);
        Assert.Equal(30, vm.Values[2].Value);
        Assert.Equal(40, vm.Values[3].Value);
        Assert.Equal(50, vm.Values[4].Value);
        Assert.Equal(60, vm.Values[5].Value);
        Assert.True(vm.Flags[0].Value);
    }

    [Fact]
    public void Constructor_WithQuadraticBezier_PopulatesValues()
    {
        var quad = new QuadraticBezier
        {
            ControlX = 10,
            ControlY = 20,
            EndX = 30,
            EndY = 40,
            IsAbsolute = true
        };

        var vm = new AddOrEditViewModel(quad, new MockDialogService());

        Assert.Equal(ItemType.QuadraticBezier, vm.Type.Type);
        Assert.Equal(10, vm.Values[0].Value);
        Assert.Equal(20, vm.Values[1].Value);
        Assert.Equal(30, vm.Values[2].Value);
        Assert.Equal(40, vm.Values[3].Value);
    }

    [Fact]
    public void Constructor_WithSmoothCubicBezier_PopulatesValues()
    {
        var scubic = new SmoothCubicBezier
        {
            Control2X = 10,
            Control2Y = 20,
            EndX = 30,
            EndY = 40,
            IsAbsolute = true
        };

        var vm = new AddOrEditViewModel(scubic, new MockDialogService());

        Assert.Equal(ItemType.SmoothCubicBezier, vm.Type.Type);
        Assert.Equal(10, vm.Values[0].Value);
        Assert.Equal(20, vm.Values[1].Value);
        Assert.Equal(30, vm.Values[2].Value);
        Assert.Equal(40, vm.Values[3].Value);
    }

    [Fact]
    public void Constructor_WithSmoothQuadraticBezier_PopulatesValues()
    {
        var squad = new SmoothQuadraticBezier
        {
            EndX = 10,
            EndY = 20,
            IsAbsolute = true
        };

        var vm = new AddOrEditViewModel(squad, new MockDialogService());

        Assert.Equal(ItemType.SmoothQuadraticBezier, vm.Type.Type);
        Assert.Equal(10, vm.Values[0].Value);
        Assert.Equal(20, vm.Values[1].Value);
    }

    [Fact]
    public void Constructor_WithEllipticalArc_PopulatesValues()
    {
        var arc = new EllipticalArc
        {
            SizeX = 10,
            SizeY = 20,
            RotationAngle = 45,
            EndX = 100,
            EndY = 200,
            IsAbsolute = true,
            IsLargeArc = true,
            IsPositiveSweepDirection = false
        };

        var vm = new AddOrEditViewModel(arc, new MockDialogService());

        Assert.Equal(ItemType.EllipticalArc, vm.Type.Type);
        Assert.Equal(10, vm.Values[0].Value);
        Assert.Equal(20, vm.Values[1].Value);
        Assert.Equal(45, vm.Values[2].Value);
        Assert.Equal(100, vm.Values[3].Value);
        Assert.Equal(200, vm.Values[4].Value);
        Assert.True(vm.Flags[0].Value);
        Assert.True(vm.Flags[1].Value);
        Assert.False(vm.Flags[2].Value);
    }

    [Fact]
    public void Constructor_WithClose_SetsType()
    {
        var close = new Close();

        var vm = new AddOrEditViewModel(close, new MockDialogService());

        Assert.Equal(ItemType.Close, vm.Type.Type);
    }
}

public class AddOrEditResultTests
{
    [Fact]
    public void Result_ReturnsCommandToString()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Move);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Flags[0].Value = true;

        Assert.Equal("M10,20", vm.Result);
    }

    [Fact]
    public void Result_UpdatesWhenValueChanges()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Move);
        vm.Values[0].Value = 10;
        vm.Values[1].Value = 20;
        vm.Flags[0].Value = true;

        var initialResult = vm.Result;
        vm.Values[0].Value = 30;

        Assert.NotEqual(initialResult, vm.Result);
        Assert.Equal("M30,20", vm.Result);
    }
}

public class AddOrEditTypeLabelTests
{
    [Fact]
    public void ValueLabels_UpdateWhenTypeChanges()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Move);

        Assert.Equal("X", vm.ValueLabels[0]);
        Assert.Equal("Y", vm.ValueLabels[1]);
        Assert.Null(vm.ValueLabels[2]);
    }

    [Fact]
    public void FlagLabels_UpdateWhenTypeChanges()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.EllipticalArc);

        Assert.Equal("Absolute Position", vm.FlagLabels[0]);
        Assert.Equal("Large Arc", vm.FlagLabels[1]);
        Assert.Equal("Positive Sweep Direction", vm.FlagLabels[2]);
    }

    [Fact]
    public void Close_HasNoLabels()
    {
        var vm = new AddOrEditViewModel(new MockDialogService());
        vm.Type = vm.ItemTypes.First(t => t.Type == ItemType.Close);

        Assert.All(vm.ValueLabels, label => Assert.Null(label));
        Assert.All(vm.FlagLabels, label => Assert.Null(label));
    }
}
