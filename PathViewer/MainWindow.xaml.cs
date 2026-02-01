using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using PathViewer.PathCommands;

namespace PathViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                viewModel?.SelectSegmentCommand.Execute(command);
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
    }
}
