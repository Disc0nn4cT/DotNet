using System.Collections.ObjectModel;
using YachtCatalog.Infrastructure;
using YachtCatalog.Models;
using YachtCatalog.Services;
namespace YachtCatalog.ViewModels;

public sealed class SavedViewModel : ObservableObject
{
    private readonly SelectionStore _store;
    public ObservableCollection<SavedSelection> Items { get; } = new();
    private SavedSelection? _selected;
    public SavedSelection? Selected
    {
        get => _selected;
        set { if (SetProperty(ref _selected, value)) { EditCommand.NotifyCanExecuteChanged(); DeleteCommand.NotifyCanExecuteChanged(); } }
    }
    private string _message = "";
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    private bool _busy;
    public bool IsBusy
    {
        get => _busy;
        private set { SetProperty(ref _busy, value); EditCommand.NotifyCanExecuteChanged(); DeleteCommand.NotifyCanExecuteChanged(); }
    }
    public AsyncCommand EditCommand { get; }
    public AsyncCommand DeleteCommand { get; }
    public SavedViewModel(SelectionStore store)
    {
        _store = store;
        EditCommand = new AsyncCommand(async () =>
        {
            if (Selected is { } row) await Shell.Current.GoToAsync($"edit?id={row.Id}");
        }, ShowError, () => Selected != null && !IsBusy);
        DeleteCommand = new AsyncCommand(async () =>
        {
            var row = Selected;
            if (row is null) return;
            if (!await Shell.Current.DisplayAlertAsync("Видалити збережену оренду?", row.ProductName, "Видалити", "Скасувати")) return;
            IsBusy = true;
            try { await _store.DeleteAsync(row.Id); }
            finally { IsBusy = false; }
            await LoadAsync();
        }, ShowError, () => Selected != null && !IsBusy);
    }
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Message = "";
        try
        {
            var rows = await _store.GetAllAsync();
            Selected = null;
            Items.Clear();
            foreach (var row in rows) Items.Add(row);
            Message = $"Збережено оренд: {Items.Count}";
        }
        catch (Exception ex) { ShowError(ex); }
        finally { IsBusy = false; }
    }
    private void ShowError(Exception ex) => Message = $"Не вдалося виконати дію: {ex.Message}";
}
