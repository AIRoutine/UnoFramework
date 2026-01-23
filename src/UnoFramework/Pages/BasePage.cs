using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Navigation;
using UnoFramework.ViewModels;

namespace UnoFramework.Pages;

/// <summary>
/// Base page class that automatically triggers PageViewModel lifecycle methods.
/// Handles both Frame navigation (OnNavigatedTo) and DataContext timing issues.
/// Calls OnNavigatedToAsync, OnNavigatingFromAsync, and OnNavigatedFromAsync on PageViewModel instances.
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
        // If we've navigated but haven't called the ViewModel method yet, do it now
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
    /// Called when the page is navigated to. Triggers PageViewModel.OnNavigatedToAsync if DataContext is a PageViewModel.
    /// If DataContext is not set yet, waits for DataContextChanged.
    /// </summary>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (DataContext is PageViewModel viewModel)
        {
            // DataContext is already set - call immediately
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
            // DataContext not set yet - wait for DataContextChanged
            _pendingNavigationEventArgs = e;
        }
    }

    /// <summary>
    /// Called when the page is navigating away. Triggers ViewModel OnNavigatingFromAsync before navigation completes.
    /// </summary>
    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);

        if (_hasNavigatedTo && DataContext is PageViewModel viewModel)
        {
            try
            {
                await viewModel.NotifyNavigatingFromAsync(e);
            }
            catch (Exception ex)
            {
                viewModel.LoggerInternal.LogError(ex, "Failed during OnNavigatingFromAsync.");
            }
        }

        // Clear pending navigation if we're leaving before DataContext was set
        _pendingNavigationEventArgs = null;
    }

    /// <summary>
    /// Called when the page is navigated away from. Triggers PageViewModel.OnNavigatedFromAsync if DataContext is a PageViewModel.
    /// </summary>
    protected override async void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);

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
