using PathViewer.PathCommands;

namespace PathViewer;

public class PathSegmentViewModel
{
    public PathCommand Command { get; set; }
    public string PathData { get; set; }
    public int Index { get; set; }

    public PathSegmentViewModel(PathCommand command, string pathData, int index)
    {
        Command = command;
        PathData = pathData;
        Index = index;
    }
}
