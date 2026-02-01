using PathViewer.PathCommands;
using Xunit;

namespace PathViewer.Tests;

public class PathCommandParseFactoryTests
{
    [Theory]
    [InlineData("M10,20", typeof(Move))]
    [InlineData("m10,20", typeof(Move))]
    [InlineData("L10,20", typeof(Line))]
    [InlineData("l10,20", typeof(Line))]
    [InlineData("H10", typeof(HorizontalLine))]
    [InlineData("h10", typeof(HorizontalLine))]
    [InlineData("V10", typeof(VerticalLine))]
    [InlineData("v10", typeof(VerticalLine))]
    [InlineData("C1,2,3,4,5,6", typeof(CubicBezier))]
    [InlineData("c1,2,3,4,5,6", typeof(CubicBezier))]
    [InlineData("S1,2,3,4", typeof(SmoothCubicBezier))]
    [InlineData("s1,2,3,4", typeof(SmoothCubicBezier))]
    [InlineData("Q1,2,3,4", typeof(QuadraticBezier))]
    [InlineData("q1,2,3,4", typeof(QuadraticBezier))]
    [InlineData("T1,2", typeof(SmoothQuadraticBezier))]
    [InlineData("t1,2", typeof(SmoothQuadraticBezier))]
    [InlineData("A10,10,0,0,0,20,20", typeof(EllipticalArc))]
    [InlineData("a10,10,0,0,0,20,20", typeof(EllipticalArc))]
    [InlineData("Z", typeof(Close))]
    [InlineData("z", typeof(Close))]
    public void Parse_RoutesToCorrectCommandType(string input, Type expectedType)
    {
        var result = PathCommand.Parse(input);

        Assert.IsType(expectedType, result);
    }

    [Fact]
    public void Parse_ThrowsForUnknownCommand()
    {
        Assert.Throws<ArgumentException>(() => PathCommand.Parse("X10,20"));
    }
}

public class MoveParseTests
{
    [Fact]
    public void Parse_AbsoluteMove()
    {
        var move = Move.Parse("M10,20");

        Assert.True(move.IsAbsolute);
        Assert.Equal(10, move.X);
        Assert.Equal(20, move.Y);
        Assert.Equal("M", move.Char);
    }

    [Fact]
    public void Parse_RelativeMove()
    {
        var move = Move.Parse("m-5,15");

        Assert.False(move.IsAbsolute);
        Assert.Equal(-5, move.X);
        Assert.Equal(15, move.Y);
        Assert.Equal("m", move.Char);
    }

    [Fact]
    public void Parse_WithSpaces()
    {
        var move = Move.Parse("M 10 20");

        Assert.Equal(10, move.X);
        Assert.Equal(20, move.Y);
    }

    [Fact]
    public void Parse_WithDecimals()
    {
        var move = Move.Parse("M10.5,20.75");

        Assert.Equal(10.5, move.X);
        Assert.Equal(20.75, move.Y);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var move = new Move { X = 10, Y = 20 };
        move.IsAbsolute = true;

        Assert.Equal("M10,20", move.ToString());
    }
}

public class LineParseTests
{
    [Fact]
    public void Parse_AbsoluteLine()
    {
        var line = Line.Parse("L100,200");

        Assert.True(line.IsAbsolute);
        Assert.Equal(100, line.EndX);
        Assert.Equal(200, line.EndY);
    }

    [Fact]
    public void Parse_RelativeLine()
    {
        var line = Line.Parse("l-10,-20");

        Assert.False(line.IsAbsolute);
        Assert.Equal(-10, line.EndX);
        Assert.Equal(-20, line.EndY);
    }
}

public class HorizontalLineParseTests
{
    [Fact]
    public void Parse_AbsoluteHorizontalLine()
    {
        var hline = HorizontalLine.Parse("H50");

        Assert.True(hline.IsAbsolute);
        Assert.Equal(50, hline.EndX);
    }

    [Fact]
    public void Parse_RelativeHorizontalLine()
    {
        var hline = HorizontalLine.Parse("h-25");

        Assert.False(hline.IsAbsolute);
        Assert.Equal(-25, hline.EndX);
    }
}

public class VerticalLineParseTests
{
    [Fact]
    public void Parse_AbsoluteVerticalLine()
    {
        var vline = VerticalLine.Parse("V75");

        Assert.True(vline.IsAbsolute);
        Assert.Equal(75, vline.EndY);
    }

    [Fact]
    public void Parse_RelativeVerticalLine()
    {
        var vline = VerticalLine.Parse("v-30");

        Assert.False(vline.IsAbsolute);
        Assert.Equal(-30, vline.EndY);
    }
}

public class CubicBezierParseTests
{
    [Fact]
    public void Parse_AbsoluteCubicBezier()
    {
        var cubic = CubicBezier.Parse("C10,20,30,40,50,60");

        Assert.True(cubic.IsAbsolute);
        Assert.Equal(10, cubic.Control1X);
        Assert.Equal(20, cubic.Control1Y);
        Assert.Equal(30, cubic.Control2X);
        Assert.Equal(40, cubic.Control2Y);
        Assert.Equal(50, cubic.EndX);
        Assert.Equal(60, cubic.EndY);
    }

    [Fact]
    public void Parse_RelativeCubicBezier()
    {
        var cubic = CubicBezier.Parse("c1,2,3,4,5,6");

        Assert.False(cubic.IsAbsolute);
        Assert.Equal(1, cubic.Control1X);
        Assert.Equal(2, cubic.Control1Y);
        Assert.Equal(3, cubic.Control2X);
        Assert.Equal(4, cubic.Control2Y);
        Assert.Equal(5, cubic.EndX);
        Assert.Equal(6, cubic.EndY);
    }

    [Fact]
    public void Parse_WithSpaces()
    {
        var cubic = CubicBezier.Parse("C 10 20 30 40 50 60");

        Assert.Equal(10, cubic.Control1X);
        Assert.Equal(20, cubic.Control1Y);
        Assert.Equal(30, cubic.Control2X);
        Assert.Equal(40, cubic.Control2Y);
        Assert.Equal(50, cubic.EndX);
        Assert.Equal(60, cubic.EndY);
    }

    [Fact]
    public void MinMax_ReturnsCorrectValues()
    {
        var cubic = CubicBezier.Parse("C10,5,30,50,20,25");

        Assert.Equal(10, cubic.MinX);
        Assert.Equal(5, cubic.MinY);
        Assert.Equal(30, cubic.MaxX);
        Assert.Equal(50, cubic.MaxY);
    }
}

public class QuadraticBezierParseTests
{
    [Fact]
    public void Parse_AbsoluteQuadraticBezier()
    {
        var quad = QuadraticBezier.Parse("Q10,20,30,40");

        Assert.True(quad.IsAbsolute);
        Assert.Equal(10, quad.ControlX);
        Assert.Equal(20, quad.ControlY);
        Assert.Equal(30, quad.EndX);
        Assert.Equal(40, quad.EndY);
    }

    [Fact]
    public void Parse_RelativeQuadraticBezier()
    {
        var quad = QuadraticBezier.Parse("q5,10,15,20");

        Assert.False(quad.IsAbsolute);
    }
}

public class EllipticalArcParseTests
{
    [Fact]
    public void Parse_AbsoluteArc()
    {
        var arc = EllipticalArc.Parse("A10,20,45,1,0,100,200");

        Assert.True(arc.IsAbsolute);
        Assert.Equal(10, arc.SizeX);
        Assert.Equal(20, arc.SizeY);
        Assert.Equal(45, arc.RotationAngle);
        Assert.True(arc.IsLargeArc);
        Assert.False(arc.IsPositiveSweepDirection);
        Assert.Equal(100, arc.EndX);
        Assert.Equal(200, arc.EndY);
    }

    [Fact]
    public void Parse_RelativeArc()
    {
        var arc = EllipticalArc.Parse("a5,5,0,0,1,10,10");

        Assert.False(arc.IsAbsolute);
        Assert.False(arc.IsLargeArc);
        Assert.True(arc.IsPositiveSweepDirection);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var arc = new EllipticalArc
        {
            SizeX = 10,
            SizeY = 20,
            RotationAngle = 0,
            IsLargeArc = true,
            IsPositiveSweepDirection = false,
            EndX = 50,
            EndY = 60
        };
        arc.IsAbsolute = true;

        Assert.Equal("A10,20,0,1,0,50,60", arc.ToString());
    }
}

public class CloseParseTests
{
    [Fact]
    public void Parse_UppercaseClose()
    {
        var close = Close.Parse("Z");

        Assert.IsType<Close>(close);
    }

    [Fact]
    public void Parse_LowercaseClose()
    {
        var close = Close.Parse("z");

        Assert.IsType<Close>(close);
    }

    [Fact]
    public void Parse_WithWhitespace()
    {
        var close = Close.Parse("  z  ");

        Assert.IsType<Close>(close);
    }

    [Fact]
    public void ToString_ReturnsZ()
    {
        var close = new Close();

        Assert.Equal("z", close.ToString());
    }
}

public class ScalePathTests
{
    [Fact]
    public void Move_ScalesCorrectly()
    {
        var move = Move.Parse("M10,20");

        move.ScalePath(2, 3);

        Assert.Equal(20, move.X);
        Assert.Equal(60, move.Y);
    }

    [Fact]
    public void Line_ScalesCorrectly()
    {
        var line = Line.Parse("L100,200");

        line.ScalePath(0.5, 0.5);

        Assert.Equal(50, line.EndX);
        Assert.Equal(100, line.EndY);
    }

    [Fact]
    public void CubicBezier_ScalesAllPoints()
    {
        var cubic = CubicBezier.Parse("C10,20,30,40,50,60");

        cubic.ScalePath(2, 2);

        Assert.Equal(20, cubic.Control1X);
        Assert.Equal(40, cubic.Control1Y);
        Assert.Equal(60, cubic.Control2X);
        Assert.Equal(80, cubic.Control2Y);
        Assert.Equal(100, cubic.EndX);
        Assert.Equal(120, cubic.EndY);
    }

    [Fact]
    public void EllipticalArc_ScalesSizeAndEnd()
    {
        var arc = EllipticalArc.Parse("A10,20,0,0,0,100,200");

        arc.ScalePath(2, 2);

        Assert.Equal(20, arc.SizeX);
        Assert.Equal(40, arc.SizeY);
        Assert.Equal(200, arc.EndX);
        Assert.Equal(400, arc.EndY);
    }
}

public class MovePathTests
{
    [Fact]
    public void Move_MovesCorrectly()
    {
        var move = Move.Parse("M10,20");

        move.MovePath(5, -5);

        Assert.Equal(15, move.X);
        Assert.Equal(15, move.Y);
    }

    [Fact]
    public void Line_MovesCorrectly()
    {
        var line = Line.Parse("L100,200");

        line.MovePath(-50, -100);

        Assert.Equal(50, line.EndX);
        Assert.Equal(100, line.EndY);
    }

    [Fact]
    public void CubicBezier_MovesAllPoints()
    {
        var cubic = CubicBezier.Parse("C10,20,30,40,50,60");

        cubic.MovePath(5, 10);

        Assert.Equal(15, cubic.Control1X);
        Assert.Equal(30, cubic.Control1Y);
        Assert.Equal(35, cubic.Control2X);
        Assert.Equal(50, cubic.Control2Y);
        Assert.Equal(55, cubic.EndX);
        Assert.Equal(70, cubic.EndY);
    }

    [Fact]
    public void EllipticalArc_MovesOnlyEndPoint()
    {
        var arc = EllipticalArc.Parse("A10,20,0,0,0,100,200");

        arc.MovePath(10, 20);

        // Size should not change
        Assert.Equal(10, arc.SizeX);
        Assert.Equal(20, arc.SizeY);
        // End point should move
        Assert.Equal(110, arc.EndX);
        Assert.Equal(220, arc.EndY);
    }
}

public class IsAbsoluteTests
{
    [Fact]
    public void SettingIsAbsolute_ChangesCharToUppercase()
    {
        var move = Move.Parse("m10,20");
        Assert.Equal("m", move.Char);

        move.IsAbsolute = true;

        Assert.Equal("M", move.Char);
    }

    [Fact]
    public void SettingIsAbsoluteFalse_ChangesCharToLowercase()
    {
        var move = Move.Parse("M10,20");
        Assert.Equal("M", move.Char);

        move.IsAbsolute = false;

        Assert.Equal("m", move.Char);
    }
}
