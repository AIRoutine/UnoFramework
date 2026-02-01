using Microsoft.Extensions.Logging;
using UnoFramework.ViewModels;

namespace UnoFramework.Controls;

/// <summary>
/// Base UserControl class that automatically triggers RegionViewModel lifecycle methods.
/// Handles both Loaded/Unloaded events and DataContext timing issues.
/// Calls OnNavigatedToAsync when the control is loaded and OnNavigatingFromAsync/OnNavigatedFromAsync when unloaded.
/// Use this for controls displayed in Regions that use RegionViewModel.
/// </summary>
public class BaseRegionControl : UserControl
{
    private bool _hasNavigatedTo;
    private bool _isLoaded;

    /// <summary>
    /// Creates a new BaseRegionControl and subscribes to Loaded/Unloaded/DataContextChanged events.
    /// </summary>
    public BaseRegionControl()
    {
        Loaded += OnLoadedAsync;
        Unloaded += OnUnloadedAsync;
        DataContextChanged += OnDataContextChangedAsync;
    }

    /// <summary>
    /// Handles DataContextChanged - triggers OnNavigatedToAsync if control is already loaded.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    private async void OnDataContextChangedAsync(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // If we're loaded but haven't called the ViewModel method yet, do it now
        if (_isLoaded && !_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync().ConfigureAwait(true);
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
    }
#pragma warning restore CA1031

    /// <summary>
    /// Called when the control is loaded. Triggers RegionViewModel.OnNavigatedToAsync if DataContext is a RegionViewModel.
    /// If DataContext is not set yet, waits for DataContextChanged.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;

        if (DataContext is RegionViewModel viewModel && !_hasNavigatedTo)
        {
            // DataContext is already set - call immediately
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync().ConfigureAwait(true);
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
        // else: DataContext not set yet - wait for DataContextChanged
    }
#pragma warning restore CA1031

    /// <summary>
    /// Called when the control is unloaded. Triggers RegionViewModel.OnNavigatedFromAsync if DataContext is a RegionViewModel.
    /// </summary>
#pragma warning disable CA1031 // Catch generic Exception in async void event handler to prevent app crash
    private async void OnUnloadedAsync(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;

        if (_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = false;
            try
            {
                await viewModel.NotifyNavigatedFromAsync().ConfigureAwait(true);
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
