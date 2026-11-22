using System.Windows.Input;
namespace YachtCatalog.Infrastructure;

public sealed class AsyncCommand(Func<Task> action, Action<Exception> onError,
    Func<bool>? canExecute = null) : ICommand
{
    private bool _running;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => !_running && (canExecute?.Invoke() ?? true);
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        _running = true;
        NotifyCanExecuteChanged();
        try { await action(); }
        catch (Exception ex) { onError(ex); }
        finally { _running = false; NotifyCanExecuteChanged(); }
    }
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
