using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Shapes;
using PathViewer.PathCommands;

namespace PathViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_MOUSEHWHEEL = 0x020E;

        private bool _isPanning;
        private Point _panStartPoint;
        private Point _panStartOffset;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Hook into the window message pump for horizontal mouse wheel (tilt)
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEHWHEEL)
            {
                // High-order word of wParam contains the wheel delta
                int delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF);

                // Scroll horizontally
                MainScrollViewer.ScrollToHorizontalOffset(
                    MainScrollViewer.HorizontalOffset + delta);

                handled = true;
            }
            return IntPtr.Zero;
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = DataContext as PathViewModel;
            if (viewModel is not null && viewModel.EditCommand.CanExecute(null))
            {
                viewModel.EditCommand.Execute(null);
            }
        }

        private void PathSegment_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path && path.Tag is PathCommand command)
            {
                var viewModel = DataContext as PathViewModel;
                if (viewModel is null) return;

                viewModel.SelectSegmentCommand.Execute(command);

                if (e.ClickCount == 2 && viewModel.EditCommand.CanExecute(null))
                {
                    viewModel.EditCommand.Execute(null);
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Check if Ctrl key is pressed
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var viewModel = DataContext as PathViewModel;
                if (viewModel is not null)
                {
                    // Adjust zoom based on wheel direction
                    double zoomDelta = e.Delta > 0 ? 0.1 : -0.1;
                    double newZoom = viewModel.Zoom + zoomDelta;

                    // Clamp zoom between 0.01 and 8.0
                    viewModel.Zoom = Math.Max(0.01, Math.Min(8.0, newZoom));

                    // Mark event as handled to prevent scrolling
                    e.Handled = true;
                }
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Middle mouse button (Button3) starts panning
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                _isPanning = true;
                _panStartPoint = e.GetPosition(MainScrollViewer);
                _panStartOffset = new Point(MainScrollViewer.HorizontalOffset, MainScrollViewer.VerticalOffset);

                // Position and show the pan indicator
                var gridPosition = e.GetPosition((Grid)MainScrollViewer.Parent);
                PanIndicator.Margin = new Thickness(
                    gridPosition.X - PanIndicator.Width / 2 + 257, // Adjust for ScrollViewer margin
                    gridPosition.Y - PanIndicator.Height / 2,
                    0, 0);
                PanIndicator.Visibility = Visibility.Visible;

                MainScrollViewer.CaptureMouse();
                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released && _isPanning)
            {
                _isPanning = false;
                PanIndicator.Visibility = Visibility.Collapsed;
                MainScrollViewer.ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                var currentPoint = e.GetPosition(MainScrollViewer);
                var delta = currentPoint - _panStartPoint;

                MainScrollViewer.ScrollToHorizontalOffset(_panStartOffset.X - delta.X);
                MainScrollViewer.ScrollToVerticalOffset(_panStartOffset.Y - delta.Y);
            }
        }
    }
}
