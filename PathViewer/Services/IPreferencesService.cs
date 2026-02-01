namespace PathViewer.Services;

public interface IPreferencesService
{
    Preferences Load();
    void Save(Preferences preferences);
}
