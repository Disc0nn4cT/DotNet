using YachtCatalog.Infrastructure;
using YachtCatalog.Models;
using YachtCatalog.Services;
namespace YachtCatalog.ViewModels;

public sealed class EditViewModel : ObservableObject
{
    private readonly SelectionStore _store;
    private SavedSelection? _row;
    private string _name = "";
    public string ProductName { get => _name; private set => SetProperty(ref _name, value); }
    private string _quantity = "1";
    public string QuantityText { get => _quantity; set => SetProperty(ref _quantity, value); }
    private string _note = "";
    public string Note { get => _note; set => SetProperty(ref _note, value); }
    private bool _include;
    public bool IncludeOption { get => _include; set => SetProperty(ref _include, value); }
    private string _optionName = "";
    public string OptionName { get => _optionName; private set => SetProperty(ref _optionName, value); }
    private string _message = "";
    public string Message { get => _message; private set => SetProperty(ref _message, value); }
    private bool _loaded;
    public bool IsLoaded { get => _loaded; private set { SetProperty(ref _loaded, value); SaveCommand.NotifyCanExecuteChanged(); } }
    public AsyncCommand SaveCommand { get; }
    public EditViewModel(SelectionStore store)
    {
        _store = store;
        SaveCommand = new AsyncCommand(SaveAsync, ex => Message = ex.Message, () => IsLoaded);
    }
    public async Task LoadAsync(int id)
    {
        IsLoaded = false;
        try
        {
            _row = await _store.GetAsync(id);
            if (_row is null) { Message = "Запис не знайдено. Поверніться до списку."; return; }
            ProductName = _row.ProductName; QuantityText = _row.Quantity.ToString();
            IncludeOption = _row.IncludeOption; Note = _row.Note; OptionName = _row.OptionName;
            Message = $"Поточна сума: {Product.Money(_row.Total)}";
            IsLoaded = true;
        }
        catch (Exception ex) { Message = $"Не вдалося відкрити запис: {ex.Message}"; }
    }
    private async Task SaveAsync()
    {
        if (_row is null) return;
        if (!int.TryParse(QuantityText, out int quantity) || quantity < 1 || quantity > Product.MaxQuantity)
        { Message = "Введіть ціле число діб від 1 до 14."; return; }
        if (Note.Length > 200) { Message = "Примітка: не більше 200 символів."; return; }
        _row.Quantity = quantity; _row.IncludeOption = IncludeOption; _row.Note = Note.Trim();
        await _store.SaveAsync(_row);
        await Shell.Current.GoToAsync("..");
    }
}
