using Shiny.Mediator;

namespace UnoFramework.Busy;

/// <summary>
/// A disposable scope that manages the global busy state via Mediator events.
/// When disposed, automatically clears the global busy state.
/// </summary>
public sealed class GlobalBusyScope : IDisposable
{
    private readonly IMediator _mediator;
    private readonly Func<bool, string?, CancellationToken, Task> _publishAction;
    private bool _disposed;

    /// <summary>
    /// Creates a new global busy scope.
    /// </summary>
    /// <param name="mediator">The mediator to publish events.</param>
    /// <param name="publishAction">Action to publish the busy event.</param>
    /// <param name="message">Optional message to display during the busy state.</param>
    internal GlobalBusyScope(IMediator mediator, Func<bool, string?, CancellationToken, Task> publishAction, string? message = null)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _publishAction = publishAction ?? throw new ArgumentNullException(nameof(publishAction));

        // Fire and forget the publish - we're in a constructor
        _ = _publishAction(true, message, CancellationToken.None);
    }

    /// <summary>
    /// Disposes the scope and clears the global busy state.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            // Fire and forget the publish
            _ = _publishAction(false, null, CancellationToken.None);
        }
    }
}
