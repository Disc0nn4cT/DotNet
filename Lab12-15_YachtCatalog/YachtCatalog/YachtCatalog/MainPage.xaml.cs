using YachtCatalog.ViewModels;

namespace YachtCatalog;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new CatalogViewModel();
    }
}
