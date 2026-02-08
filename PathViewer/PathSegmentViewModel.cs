using PathViewer.PathCommands;

namespace PathViewer;

public class PathSegmentViewModel
{
    public PathCommand Command { get; set; }
    public string PathData { get; set; }
    public int Index { get; set; }

    // Node indicator for non-drawing commands (Move, Close)
    public bool ShowNode { get; set; }
    public double NodeX { get; set; }
    public double NodeY { get; set; }

    public PathSegmentViewModel(PathCommand command, string pathData, int index)
    {
        Command = command;
        PathData = pathData;
        Index = index;
    }
}
