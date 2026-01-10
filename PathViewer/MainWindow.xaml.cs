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

        private void PathSegment_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Path path && path.Tag is PathCommand command)
            {
                var viewModel = DataContext as PathViewModel;
                viewModel?.SelectSegmentCommand.Execute(command);
            }
        }
    }
}
