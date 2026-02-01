using Microsoft.Extensions.Logging;
using UnoFramework.ViewModels;

namespace UnoFramework.Pages;

/// <summary>
/// Base page class that automatically triggers PageViewModel lifecycle methods.
/// Handles both Frame navigation (OnNavigatedTo/OnNavigatedFrom) and DataContext timing issues.
/// </summary>
public class BasePage : Page
{
    private bool _hasNavigatedTo;
    private NavigationEventArgs? _pendingNavigationEventArgs;

    /// <summary>
    /// Creates a new BasePage and subscribes to DataContextChanged.
    /// </summary>
    public BasePage() => DataContextChanged += OnDataContextChangedAsync;

    /// <summary>
    /// Handles DataContextChanged - triggers OnNavigatedToAsync if we've already navigated.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    private async void OnDataContextChangedAsync(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (_pendingNavigationEventArgs != null && !_hasNavigatedTo && DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync(_pendingNavigationEventArgs).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                // Navigation was cancelled - expected behavior
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedToAsync.");
            }
            finally
            {
                _pendingNavigationEventArgs = null;
            }
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// Called when the page is navigated to. Triggers PageViewModel.OnNavigatedToAsync.
    /// If DataContext is not set yet, waits for DataContextChanged.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync(e).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                // Navigation was cancelled - expected behavior
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedToAsync.");
            }
        }
        else
        {
            _pendingNavigationEventArgs = e;
        }
    }
#pragma warning restore CA1031

    /// <summary>
    /// Called when the page is navigated away from. Triggers PageViewModel.OnNavigatedFromAsync.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        _pendingNavigationEventArgs = null;

        if (_hasNavigatedTo && DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = false;
            try
            {
                await viewModel.NotifyNavigatedFromAsync(e).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                // Navigation was cancelled - expected behavior
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedFromAsync.");
            }
        }
    }
#pragma warning restore CA1031
}
