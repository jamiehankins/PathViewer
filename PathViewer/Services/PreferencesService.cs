namespace PathViewer.Services;

public class PreferencesService : IPreferencesService
{
    public Preferences Load() => Preferences.Load();

    public void Save(Preferences preferences) => preferences.Save();
}
