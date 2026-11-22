using YachtCatalog.Pages;
namespace YachtCatalog;
public partial class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        InitializeComponent();
        CatalogContent.Content = mainPage;
        Routing.RegisterRoute("saved", typeof(SavedPage));
        Routing.RegisterRoute("edit", typeof(EditPage));
    }
}
