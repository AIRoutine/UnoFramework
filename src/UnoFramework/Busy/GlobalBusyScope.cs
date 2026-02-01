namespace UnoFramework.Busy;

/// <summary>
/// A disposable scope that manages the global busy state via Mediator events.
/// When disposed, automatically clears the global busy state.
/// </summary>
public sealed class GlobalBusyScope : IDisposable
{
    private readonly Func<bool, string?, CancellationToken, Task> _publishAction;
    private bool _disposed;

    /// <summary>
    /// Creates a new global busy scope.
    /// </summary>
    /// <param name="publishAction">Action to publish the busy event.</param>
    /// <param name="message">Optional message to display during the busy state.</param>
    internal GlobalBusyScope(Func<bool, string?, CancellationToken, Task> publishAction, string? message = null)
    {
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
