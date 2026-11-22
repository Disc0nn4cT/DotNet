using YachtCatalog.ViewModels;
namespace YachtCatalog.Pages;
public partial class SavedPage : ContentPage
{
    private readonly SavedViewModel _vm;
    public SavedPage(SavedViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
