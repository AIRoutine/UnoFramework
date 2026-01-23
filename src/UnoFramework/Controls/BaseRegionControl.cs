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
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
    }

    /// <summary>
    /// Handles DataContextChanged - triggers OnNavigatedToAsync if control is already loaded.
    /// </summary>
    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // If we're loaded but haven't called the ViewModel method yet, do it now
        if (_isLoaded && !_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync();
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedToAsync.");
            }
        }
    }

    /// <summary>
    /// Called when the control is loaded. Triggers RegionViewModel.OnNavigatedToAsync if DataContext is a RegionViewModel.
    /// If DataContext is not set yet, waits for DataContextChanged.
    /// </summary>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;

        if (DataContext is RegionViewModel viewModel && !_hasNavigatedTo)
        {
            // DataContext is already set - call immediately
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync();
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedToAsync.");
            }
        }
        // else: DataContext not set yet - wait for DataContextChanged
    }

    /// <summary>
    /// Called when the control is unloaded. Triggers RegionViewModel.OnNavigatedFromAsync if DataContext is a RegionViewModel.
    /// </summary>
    private async void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;

        if (_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = false;
            try
            {
                await viewModel.NotifyNavigatedFromAsync();
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedFromAsync.");
            }
        }
    }
}
