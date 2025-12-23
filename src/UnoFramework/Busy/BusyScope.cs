using UnoFramework.ViewModels;

namespace UnoFramework.Busy;

/// <summary>
/// A disposable scope that manages the busy state of a ViewModel.
/// When disposed, automatically clears the busy state.
/// </summary>
public sealed class BusyScope : IDisposable
{
    private readonly ViewModelBase _viewModel;
    private bool _disposed;

    /// <summary>
    /// Creates a new busy scope for the specified ViewModel.
    /// </summary>
    /// <param name="viewModel">The ViewModel to manage.</param>
    /// <param name="message">Optional message to display during the busy state.</param>
    public BusyScope(ViewModelBase viewModel, string? message = null)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _viewModel.IsBusy = true;
        _viewModel.BusyMessage = message;
    }

    /// <summary>
    /// Updates the busy message while within the scope.
    /// </summary>
    /// <param name="message">The new message to display.</param>
    public void UpdateMessage(string? message)
    {
        if (!_disposed)
        {
            _viewModel.BusyMessage = message;
        }
    }

    /// <summary>
    /// Disposes the scope and clears the busy state.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _viewModel.IsBusy = false;
            _viewModel.BusyMessage = null;
        }
    }
}
