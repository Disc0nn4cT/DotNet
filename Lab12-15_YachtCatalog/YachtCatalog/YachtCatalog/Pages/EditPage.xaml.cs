using YachtCatalog.ViewModels;
namespace YachtCatalog.Pages;
public partial class EditPage : ContentPage, IQueryAttributable
{
    private readonly EditViewModel _vm;
    private int _id;
    public EditPage(EditViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _id = query.TryGetValue("id", out var value) && int.TryParse(value?.ToString(), out int id) ? id : 0;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(_id);
    }
}
