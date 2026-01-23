using CommunityToolkit.Mvvm.ComponentModel;
using UnoFramework.Busy;
using UnoFramework.Contracts.Busy;
using Microsoft.Extensions.Logging;
using Shiny.Mediator;
using UnoFramework.Contracts.Navigation;

namespace UnoFramework.ViewModels;

/// <summary>
/// Base class for all ViewModels providing logging, mediator, and busy state.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    private CancellationTokenSource? _navigationCts;
    private readonly object _initializeLock = new();
    private Task? _initializeTask;

    /// <summary>
    /// CancellationToken that is cancelled when navigating away from the ViewModel.
    /// Use this token for async operations that should be cancelled on navigation.
    /// </summary>
    protected CancellationToken NavigationToken => _navigationCts?.Token ?? CancellationToken.None;

    /// <summary>
    /// Called when navigating to this ViewModel. Creates a new CancellationTokenSource.
    /// </summary>
    protected virtual void OnNavigatingTo()
    {
        if (_navigationCts != null)
        {
            _navigationCts.Cancel();
            _navigationCts.Dispose();
        }
        _navigationCts = new CancellationTokenSource();
    }

    /// <summary>
    /// Called asynchronously when navigating to this ViewModel.
    /// Override this to perform async operations when the ViewModel becomes active.
    /// </summary>
    protected virtual Task OnNavigatingToAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when navigating away from this ViewModel. Cancels pending operations and resets busy state.
    /// </summary>
    protected virtual void OnNavigatingFrom()
    {
        if (_navigationCts != null)
        {
            _navigationCts.Cancel();
            _navigationCts.Dispose();
            _navigationCts = null;
        }

        IsBusy = false;
        BusyMessage = null;
    }

    /// <summary>
    /// Called asynchronously when navigating away from this ViewModel.
    /// Override this to perform cleanup operations before the ViewModel becomes inactive.
    /// </summary>
    protected virtual Task OnNavigatingFromAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// The logger instance for this ViewModel.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Internal logger access for framework components.
    /// </summary>
    internal ILogger LoggerInternal => Logger;

    /// <summary>
    /// The mediator for publishing events.
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// The navigator for page navigation.
    /// </summary>
    protected INavigator Navigator { get; }

    /// <summary>
    /// The route notifier for tracking navigation changes.
    /// </summary>
    protected IRouteNotifier RouteNotifier { get; }

    /// <summary>
    /// Creates a new ViewModelBase with base services.
    /// </summary>
    /// <param name="baseServices">Common services for all ViewModels.</param>
    protected ViewModelBase(BaseServices baseServices)
    {
        ArgumentNullException.ThrowIfNull(baseServices);

        Logger = baseServices.LoggerFactory.CreateLogger(GetType());
        Mediator = baseServices.Mediator;
        Navigator = baseServices.Navigator;
        RouteNotifier = baseServices.RouteNotifier;
    }

    /// <summary>
    /// Override this method to perform async initialization on first navigation (lazy).
    /// This is called once on the first navigation to the ViewModel.
    /// </summary>
    protected virtual Task InitializeAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Ensures InitializeAsync is run once. Retries if the previous attempt faulted or was canceled.
    /// </summary>
    protected Task EnsureInitializedAsync(CancellationToken ct = default)
    {
        lock (_initializeLock)
        {
            if (_initializeTask == null || _initializeTask.IsCanceled || _initializeTask.IsFaulted)
            {
                _initializeTask = InitializeAsync(ct);
            }

            return _initializeTask;
        }
    }

    /// <summary>
    /// Indicates whether the ViewModel is currently busy (e.g., loading data).
    /// </summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>
    /// Optional message to display while the ViewModel is busy.
    /// </summary>
    [ObservableProperty]
    private string? _busyMessage;

    /// <summary>
    /// Begins a busy scope. Use with 'using' statement for automatic cleanup.
    /// </summary>
    /// <param name="message">Optional message to display during the busy state.</param>
    /// <returns>A disposable scope that automatically clears the busy state when disposed.</returns>
    protected BusyScope BeginBusy(string? message = null)
    {
        return new BusyScope(this, message);
    }

    /// <summary>
    /// Begins a global busy scope. Use with 'using' statement for automatic cleanup.
    /// Publishes GlobalBusyEvent via Mediator.
    /// </summary>
    /// <param name="message">Optional message to display during the busy state.</param>
    /// <returns>A disposable scope that automatically clears the global busy state when disposed.</returns>
    protected GlobalBusyScope BeginGlobalBusy(string? message = null)
    {
        return new GlobalBusyScope(Mediator, PublishGlobalBusyAsync, message);
    }

    private Task PublishGlobalBusyAsync(bool isBusy, string? message, CancellationToken ct)
    {
        return Mediator.Publish(new GlobalBusyEvent(isBusy, message), ct);
    }

    /// <summary>
    /// Sets the busy state with an optional message.
    /// </summary>
    /// <param name="isBusy">Whether the ViewModel is busy.</param>
    /// <param name="message">Optional message to display.</param>
    protected void SetBusy(bool isBusy, string? message = null)
    {
        IsBusy = isBusy;
        BusyMessage = isBusy ? message : null;
    }
}
