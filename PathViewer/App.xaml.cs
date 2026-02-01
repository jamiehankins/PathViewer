using System.Windows;

namespace PathViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Load and apply saved theme
            var prefs = Preferences.Load();
            SetTheme(prefs.Theme);
        }

        public void SetTheme(string theme)
        {
#pragma warning disable WPF0001
            ThemeMode = theme switch
            {
                "Light" => ThemeMode.Light,
                "Dark" => ThemeMode.Dark,
                _ => ThemeMode.System
            };
#pragma warning restore WPF0001
        }
    }
}
