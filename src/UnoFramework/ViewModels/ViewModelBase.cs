using CommunityToolkit.Mvvm.ComponentModel;
using UnoFramework.Busy;
using UnoFramework.Contracts.Busy;
using Microsoft.Extensions.Logging;
using Shiny.Mediator;

namespace UnoFramework.ViewModels;

/// <summary>
/// Base class for all ViewModels providing logging, mediator, and busy state.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    /// <summary>
    /// The logger instance for this ViewModel.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The mediator for publishing events.
    /// </summary>
    protected IMediator Mediator { get; }

    /// <summary>
    /// The navigator for page navigation.
    /// </summary>
    protected INavigator Navigator { get; }

    /// <summary>
    /// Creates a new ViewModelBase with base services.
    /// </summary>
    /// <param name="baseServices">Common services for all ViewModels.</param>
    protected ViewModelBase(BaseServices baseServices)
    {
        Logger = baseServices.LoggerFactory.CreateLogger(GetType());
        Mediator = baseServices.Mediator;
        Navigator = baseServices.Navigator;
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
