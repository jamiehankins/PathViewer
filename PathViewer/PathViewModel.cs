using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PathViewer.PathCommands;
using PathViewer.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace PathViewer;

public partial class PathViewModel : ViewModelBase
{
    private readonly IPreferencesService _preferencesService;

    // Explicit parameterless constructor for XAML instantiation
    public PathViewModel() : this(null, null) { }

    public PathViewModel(
        IDialogService? dialogService,
        IPreferencesService? preferencesService)
        : base(dialogService)
    {
        _preferencesService = preferencesService ?? new PreferencesService();

        // Design mode: use sample data, skip runtime initialization
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            ParseData(Data);
            Bounds = Geometry.Parse(Data).Bounds;
            return;
        }

        // CommunityToolkit.MVVM doesn't subscribe to RequestSuggested,
        // so that makes change notifications a manual thing. This subverts that.
        FindRelayCommands();

        // Load preferences
        LoadPreferences();

        ParseData(Data);
        Bounds = Geometry.Parse(Data).Bounds;
    }

    private void LoadPreferences()
    {
        var prefs = _preferencesService.Load();
        Theme = prefs.Theme;
        StrokeColor = prefs.StrokeColor;
        StrokeThickness = (int)prefs.StrokeThickness;
        FillColor = prefs.FillColor;
        ShowOriginLines = prefs.ShowOriginLines;
        ShowBoundingBox = prefs.ShowBoundingBox;
    }

    private void SavePreferences()
    {
        var prefs = new Preferences
        {
            Theme = Theme,
            StrokeColor = StrokeColor,
            StrokeThickness = StrokeThickness,
            FillColor = FillColor,
            ShowOriginLines = ShowOriginLines,
            ShowBoundingBox = ShowBoundingBox
        };
        _preferencesService.Save(prefs);
    }

    // Generate individual path strings up to a specific command
    public string GetPathUpToIndex(int index)
    {
        if (index < 0 || index >= PathCommands.Count)
            return string.Empty;
        
        return string.Join(" ", PathCommands.Take(index + 1).Select(p => p.ToString()));
    }

    // Get the starting position for a command at a given index
    private (double x, double y) GetStartingPosition(int index)
    {
        double currentX = 0, currentY = 0;
        
        for (int i = 0; i < index; i++)
        {
            var cmd = PathCommands[i];
            UpdatePosition(cmd, ref currentX, ref currentY);
        }
        
        return (currentX, currentY);
    }

    // Update current position based on a command
    private void UpdatePosition(PathCommand cmd, ref double currentX, ref double currentY)
    {
        if (cmd is PathCommands.Move move)
        {
            if (move.IsAbsolute)
            {
                currentX = move.X;
                currentY = move.Y;
            }
            else
            {
                currentX += move.X;
                currentY += move.Y;
            }
        }
        else if (cmd is PathCommands.Line line)
        {
            if (line.IsAbsolute)
            {
                currentX = line.EndX;
                currentY = line.EndY;
            }
            else
            {
                currentX += line.EndX;
                currentY += line.EndY;
            }
        }
        else if (cmd is PathCommands.HorizontalLine hline)
        {
            if (hline.IsAbsolute)
                currentX = hline.EndX;
            else
                currentX += hline.EndX;
        }
        else if (cmd is PathCommands.VerticalLine vline)
        {
            if (vline.IsAbsolute)
                currentY = vline.EndY;
            else
                currentY += vline.EndY;
        }
        else if (cmd is PathCommands.CubicBezier cubic)
        {
            if (cubic.IsAbsolute)
            {
                currentX = cubic.EndX;
                currentY = cubic.EndY;
            }
            else
            {
                currentX += cubic.EndX;
                currentY += cubic.EndY;
            }
        }
        else if (cmd is PathCommands.QuadraticBezier quad)
        {
            if (quad.IsAbsolute)
            {
                currentX = quad.EndX;
                currentY = quad.EndY;
            }
            else
            {
                currentX += quad.EndX;
                currentY += quad.EndY;
            }
        }
        else if (cmd is PathCommands.SmoothCubicBezier scubic)
        {
            if (scubic.IsAbsolute)
            {
                currentX = scubic.EndX;
                currentY = scubic.EndY;
            }
            else
            {
                currentX += scubic.EndX;
                currentY += scubic.EndY;
            }
        }
        else if (cmd is PathCommands.SmoothQuadraticBezier squad)
        {
            if (squad.IsAbsolute)
            {
                currentX = squad.EndX;
                currentY = squad.EndY;
            }
            else
            {
                currentX += squad.EndX;
                currentY += squad.EndY;
            }
        }
        else if (cmd is PathCommands.EllipticalArc arc)
        {
            if (arc.IsAbsolute)
            {
                currentX = arc.EndX;
                currentY = arc.EndY;
            }
            else
            {
                currentX += arc.EndX;
                currentY += arc.EndY;
            }
        }
    }

    // Get individual path data for a specific command
    public string GetIndividualPathData(int index)
    {
        if (index < 0 || index >= PathCommands.Count)
            return string.Empty;
        
        var (startX, startY) = GetStartingPosition(index);
        var command = PathCommands[index].ToString();
        return $"M{startX},{startY} {command}";
    }

    [RelayCommand]
    private void SelectSegment(object parameter)
    {
        if (parameter is PathCommand cmd)
        {
            SelectedIndex = PathCommands.IndexOf(cmd);
        }
    }

    private void ParseData(string data)
    {
        PathCommands.Clear();
        Match match = _regex.Match(data);
        while (match.Success)
        {
            try
            {
                PathCommands.Add(PathCommand.Parse(match.Groups[0].Value));
            }
            catch (Exception ex)
            {
                PathError = ex.Message;
                return;
            }
            match = match.NextMatch();
        }
        RefreshPathSegments();
    }

    private static Regex _regex = new(@"([A-Za-z])(?:\s*([+\-]?\d*\.?\d+(?:[eE][+\-]?\d+)?)(?:\s*,\s*|\s+|\b(?![,\.\s])))*");

    #region Commands
    [RelayCommand(CanExecute = nameof(CanReset))]
    private void ResetZoom() => Zoom = 1;

    [RelayCommand]
    private void AddItem() => AddItemAt(-1);

    private void AddItemAt(int location = -1)
    {
        using AddOrEditViewModel add = new();
        if (ShowModal(
            add,
            (location == -1 ? "Insert" : "Add") + " Path Command"))
        {
            if (location != -1)
            {
                PathCommands.Insert(location, add.Command);
            }
            else
            {
                PathCommands.Add(add.Command);
            }
            Data = GeneratedData;
            RefreshPathSegments();
        }
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void InsertItem()
    {
        if (SelectedIndex == -1) return;
        AddItemAt(SelectedIndex);
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void Edit()
    {
        if (SelectedItem is null) return;
        using AddOrEditViewModel edit = new(SelectedItem);
        if (ShowModal(edit, "Edit Path Command"))
        {
            // Easier to insert new (edited) item than
            // copy properties.
            int target = SelectedIndex + 1;
            PathCommands.Insert(SelectedIndex, edit.Command);
            PathCommands.RemoveAt(target);
            SelectedIndex = target;

            Data = GeneratedData;
            RefreshPathSegments();
        }
    }

    [RelayCommand(CanExecute = nameof(SomethingSelected))]
    private void Delete()
    {
        if (SelectedItem is null) return;
        int target = SelectedIndex;
        PathCommands.RemoveAt(target);
        if (PathCommands.Count > 0)
        {
            SelectedIndex =
                PathCommands.Count >= target - 1
                ? target
                : PathCommands.Count - 1;
        }
        Data = GeneratedData;
        RefreshPathSegments();
    }

    [RelayCommand(CanExecute = nameof(HasCommands))]
    private void Clear()
    {
        PathCommands.Clear();

        Data = GeneratedData;
        RefreshPathSegments();
    }

    [RelayCommand(CanExecute = nameof(CanMoveUp))]
    private void MoveUp()
    {
        if (SelectedItem is null) return;
        PathCommand cmd = SelectedItem;
        int target = SelectedIndex - 1;
        PathCommands.RemoveAt(SelectedIndex);
        PathCommands.Insert(target, cmd);
        SelectedIndex = target;

        Data = GeneratedData;
        RefreshPathSegments();
    }

    [RelayCommand(CanExecute = nameof(CanMoveDown))]
    private void MoveDown()
    {
        if (SelectedItem is null) return;
        PathCommand cmd = SelectedItem;
        int target = SelectedIndex + 1;
        PathCommands.RemoveAt(SelectedIndex);
        PathCommands.Insert(target, cmd);
        SelectedIndex = target;

        Data = GeneratedData;
        RefreshPathSegments();
    }

    [RelayCommand]
    private void ScalePath()
    {
        ScaleOrMoveViewModel vm = new(PathWidth, PathHeight, true);
        if (ShowModal(vm, "ScalePath Path"))
        {
            double scaleX = vm.Width / PathWidth;
            double scaleY = vm.Height / PathHeight;
            foreach (PathCommand cmd in PathCommands)
            {
                cmd.ScalePath(scaleX, scaleY);
            }
            Data = GeneratedData;
            RefreshPathSegments();
        }
    }

    [RelayCommand]
    private void MovePath()
    {
        ScaleOrMoveViewModel vm = new(-Origin.X, -Origin.Y, false);
        if (ShowModal(vm, "ScalePath Path"))
        {
            foreach (PathCommand cmd in PathCommands)
            {
                cmd.MovePath(vm.Width, vm.Height);
            }
            Data = GeneratedData;
            RefreshPathSegments();
        }
    }

    #region Predicates
    private bool CanReset => Zoom != 1;
    private bool SomethingSelected => SelectedIndex != -1;
    private bool HasCommands => PathCommands.Count > 0;
    private bool CanMoveUp => SelectedIndex > 0;
    private bool CanMoveDown => SelectedIndex >= 0 && SelectedIndex < PathCommands.Count - 1;
    #endregion
    #endregion

    #region Helpers
    private string GeneratedData => string.Join(" ", PathCommands.Select(p => p.ToString()));

    private bool _isUpdating;
    #endregion

    #region View Model Properties
    private string _data =
        "M-50,90 "
        + "A90,90 0 0 0 180,90 "
        + "M30,30 "
        + "A15,15 0 0 0 60,30 "
        + "M30,30 "
        + "A15,15 0 0 1 60,30 "
        + "M120,30 "
        + "A15,15 0 0 0 150,30 "
        + "M120,30 "
        + "A15,15 0 0 1 150,30";
    public string Data
    {
        get => _data;
        set
        {
            if (!_isUpdating && SetProperty(ref _data, value))
            {
                try
                {
                    int curSel = SelectedIndex;
                    _isUpdating = true;
                    Bounds = Geometry.Parse(_data).Bounds;
                    PathError = null;
                    ParseData(_data);
                    SelectedIndex = curSel;
                }
                catch (FormatException ex)
                {
                    PathError = ex.Message;
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }
    }

    public List<string> ThemesList => new() { "System", "Light", "Dark" };

    private string _theme = "System";
    public string Theme
    {
        get => _theme;
        set
        {
            if (SetProperty(ref _theme, value))
            {
                ApplyTheme(value);
                SavePreferences();
            }
        }
    }

    private void ApplyTheme(string theme)
    {
        if (Application.Current is App app)
        {
            app.SetTheme(theme);
        }
    }

    private bool _showOriginLines = true;
    public bool ShowOriginLines
    {
        get => _showOriginLines;
        set
        {
            if (SetProperty(ref _showOriginLines, value))
            {
                SavePreferences();
            }
        }
    }

    private bool _showBoundingBox = true;
    public bool ShowBoundingBox
    {
        get => _showBoundingBox;
        set
        {
            if (SetProperty(ref _showBoundingBox, value))
            {
                SavePreferences();
            }
        }
    }

    [ObservableProperty]
    private double _zoom = 1.0;

    public List<string> ColorsList => typeof(Colors).GetProperties().Select(p => p.Name).ToList();

    public static List<int> Thicknesses => Enumerable.Range(1, 20).ToList();

    private string _strokeColor = nameof(Colors.Black);
    public string StrokeColor
    {
        get => _strokeColor;
        set
        {
            if (SetProperty(ref _strokeColor, value))
            {
                SavePreferences();
            }
        }
    }

    private int _strokeThickness = 2;
    public int StrokeThickness
    {
        get => _strokeThickness;
        set
        {
            if (SetProperty(ref _strokeThickness, value))
            {
                SavePreferences();
                OnPropertyChanged(nameof(HighlightStrokeThickness));
            }
        }
    }

    public double HighlightStrokeThickness => StrokeThickness + 4;

    private string _fillColor = nameof(Colors.Transparent);
    public string FillColor
    {
        get => _fillColor;
        set
        {
            if (SetProperty(ref _fillColor, value))
            {
                SavePreferences();
            }
        }
    }

    public ObservableCollection<PathCommand> PathCommands { get; } = new();

    public ObservableCollection<PathSegmentViewModel> PathSegments { get; } = new();

    private void RefreshPathSegments()
    {
        PathSegments.Clear();
        for (int i = 0; i < PathCommands.Count; i++)
        {
            var pathData = GetIndividualPathData(i);
            PathSegments.Add(new PathSegmentViewModel(PathCommands[i], pathData, i));
        }
    }

    private PathCommand? _selectedItem;
    public PathCommand? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (_selectedItem is not null)
            {
                _selectedItem.IsSelected = false;
            }
            if (SetProperty(ref _selectedItem, value) && value is not null)
            {
                value.IsSelected = true;
            }
            OnPropertyChanged(nameof(SelectedPathData));
        }
    }

    // Path data for highlighting the selected segment only
    public string SelectedPathData
    {
        get
        {
            if (SelectedIndex < 0 || SelectedIndex >= PathCommands.Count)
                return string.Empty;
            return GetIndividualPathData(SelectedIndex);
        }
    }

    [ObservableProperty]
    private int _selectedIndex = -1;

    partial void OnSelectedIndexChanged(int value)
    {
        SelectedItem = (value >= 0 && value < PathCommands.Count) ? PathCommands[value] : null;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanvasSize))]
    private Size _viewportSize;

    public Size CanvasSize =>
        new(Math.Max(ViewportSize.Width, Bounds.Width + PathMargin * 2),
            Math.Max(ViewportSize.Height, Bounds.Height + PathMargin * 2));

    [ObservableProperty]
    private double _pathMargin = 20;

    [ObservableProperty]
    private string? _pathError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PathWidth))]
    [NotifyPropertyChangedFor(nameof(PathHeight))]
    [NotifyPropertyChangedFor(nameof(Origin))]
    [NotifyPropertyChangedFor(nameof(Extent))]
    [NotifyPropertyChangedFor(nameof(PathStartX))]
    [NotifyPropertyChangedFor(nameof(PathStartY))]
    [NotifyPropertyChangedFor(nameof(CanvasSize))]
    [NotifyPropertyChangedFor(nameof(Size))]
    [NotifyPropertyChangedFor(nameof(ZeroOrigin))]
    [NotifyPropertyChangedFor(nameof(TopLabel))]
    [NotifyPropertyChangedFor(nameof(BottomLabel))]
    [NotifyPropertyChangedFor(nameof(LeftLabel))]
    [NotifyPropertyChangedFor(nameof(RightLabel))]
    private Rect _bounds;

    public double PathWidth => Bounds.Width;

    public double PathHeight => Bounds.Height;

    public Size Size => Bounds.Size;

    public Point Origin => Bounds.TopLeft;

    public Point Extent => Bounds.BottomRight;

    public double DrawWidth => Bounds.Width + PathMargin * 2;
    public double DrawHeight => Bounds.Height + PathMargin * 2;

    public double PathStartX => -Bounds.X + PathMargin;

    public double PathStartY => -Bounds.Y + PathMargin;

    public Point ZeroOrigin => new(-Bounds.X, -Bounds.Y);

    public double LeftLabel => PathMargin / 2;
    public double RightLabel => Bounds.Width + PathMargin * 1.5;
    public double TopLabel => PathMargin / 2;
    public double BottomLabel => Bounds.Height + PathMargin * 1.5;
    #endregion
}
