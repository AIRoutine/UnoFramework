using UnoFramework.ViewModels;

namespace UnoFramework.Controls;

/// <summary>
/// Base UserControl class that automatically triggers RegionViewModel lifecycle methods.
/// Calls OnNavigatedToAsync when the control is loaded and OnNavigatedFromAsync when unloaded.
/// Use this for controls displayed in Regions that use RegionViewModel.
/// </summary>
public class BaseRegionControl : UserControl
{
    private bool _hasNavigatedTo;

    /// <summary>
    /// Creates a new BaseRegionControl and subscribes to Loaded/Unloaded events.
    /// </summary>
    public BaseRegionControl()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Called when the control is loaded. Triggers RegionViewModel.OnNavigatedToAsync if DataContext is a RegionViewModel.
    /// </summary>
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = true;
            await viewModel.OnNavigatedToAsync();
        }
    }

    /// <summary>
    /// Called when the control is unloaded. Triggers RegionViewModel.OnNavigatedFromAsync if DataContext is a RegionViewModel.
    /// </summary>
    private async void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_hasNavigatedTo && DataContext is RegionViewModel viewModel)
        {
            _hasNavigatedTo = false;
            await viewModel.OnNavigatedFromAsync();
        }
    }
}
