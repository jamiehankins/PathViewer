using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PathViewer.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;


namespace PathViewer;

public class ViewModelBase : ObservableObject, IDisposable
{
    protected readonly IDialogService _dialogService;

    public ViewModelBase(IDialogService? dialogService = null)
    {
        _dialogService = dialogService ?? new DialogService();

        // Need the null check for design mode.
        if (Application.Current?.MainWindow is not null)
        {
            Application.Current.MainWindow.Closing += OnMainWindowClosing;
        }
    }

    public virtual void Dispose()
    {
        CommandManager.RequerySuggested -= OnRequerySuggested;
        if (Application.Current?.MainWindow is not null)
        {
            Application.Current.MainWindow.Closing -= OnMainWindowClosing;
        }
    }

    protected bool ShowModal(ViewModelBase vm, string title) => _dialogService.ShowModal(vm, title);

    protected virtual void OnMainWindowClosing(object? sender, CancelEventArgs e)
    {
        // Checking e.Cancel just in case there's ever a dialog to make sure.
        if (!e.Cancel)
        {
            Dispose();
        }
    }

    #region Close Notification
    public event CancelEventHandler? CloseEvent;
    protected void OnClose(bool cancel = false) => CloseEvent?.Invoke(this, new CancelEventArgs { Cancel = cancel });
    #endregion


    protected void FindRelayCommands()
    {
        // CommunityToolkit doesn't subscribe to CommandManager.RequerySuggested,
        // and we need those notifications to go to our commands.
        CommandManager.RequerySuggested += OnRequerySuggested;
        foreach (PropertyInfo cmdInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var obj = cmdInfo.GetValue(this);
            if (obj is IRelayCommand cmd) _commands.Add(cmd);
        }
    }

    private void OnRequerySuggested(object? sender, EventArgs e)
    {
        foreach (IRelayCommand command in _commands)
        {
            command.NotifyCanExecuteChanged();
        }
    }
    private readonly List<IRelayCommand> _commands = new();

    protected static readonly string _eol = Environment.NewLine;
}
