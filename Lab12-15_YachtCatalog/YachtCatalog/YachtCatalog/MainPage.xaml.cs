using YachtCatalog.ViewModels;
namespace YachtCatalog;
public partial class MainPage : ContentPage
{
    public MainPage(CatalogViewModel vm) { InitializeComponent(); BindingContext = vm; }
    private bool _alternateAccent;
    private void OnAccentClicked(object? sender, EventArgs e)
    {
        _alternateAccent = !_alternateAccent;
        Application.Current!.Resources["AccentColor"] = Color.FromArgb(_alternateAccent ? "#0E6E6E" : "#0B2F4A");
    }
}
