using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Navigation;
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
    public BasePage()
    {
        DataContextChanged += OnDataContextChanged;
    }

    /// <summary>
    /// Handles DataContextChanged - triggers OnNavigatedToAsync if we've already navigated.
    /// </summary>
    private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        if (_pendingNavigationEventArgs != null && !_hasNavigatedTo && DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync(_pendingNavigationEventArgs);
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

    /// <summary>
    /// Called when the page is navigated to. Triggers PageViewModel.OnNavigatedToAsync.
    /// If DataContext is not set yet, waits for DataContextChanged.
    /// </summary>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = true;
            try
            {
                await viewModel.NotifyNavigatedToAsync(e);
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

    /// <summary>
    /// Called when the page is navigated away from. Triggers PageViewModel.OnNavigatedFromAsync.
    /// </summary>
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

        _pendingNavigationEventArgs = null;

        if (_hasNavigatedTo && DataContext is PageViewModel viewModel)
        {
            _hasNavigatedTo = false;
            try
            {
                await viewModel.NotifyNavigatedFromAsync(e);
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatedFromAsync.");
            }
        }
    }
}
