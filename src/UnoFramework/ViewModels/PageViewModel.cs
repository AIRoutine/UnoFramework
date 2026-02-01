namespace UnoFramework.ViewModels;

/// <summary>
/// ViewModel for Frame-based page navigation with lifecycle support.
/// Provides OnNavigatedToAsync and OnNavigatedFromAsync methods that are called
/// from the Page's OnNavigatedTo and OnNavigatedFrom events.
/// </summary>
public abstract partial class PageViewModel(BaseServices baseServices) : ViewModelBase(baseServices)
{
    /// <summary>
    /// Called when the page is navigated to.
    /// Override this method to load data or perform initialization when the page becomes active.
    /// </summary>
    /// <param name="e">Navigation event arguments containing mode and parameter.</param>
    /// <param name="ct">Cancellation token that is cancelled when navigating away.</param>
    protected virtual Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default) =>
        Task.CompletedTask;

    /// <summary>
    /// Called when the page is navigated away from.
    /// Override this method to save state or perform cleanup.
    /// </summary>
    /// <param name="e">Navigation event arguments.</param>
    /// <param name="ct">Cancellation token.</param>
    protected virtual Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default) =>
        Task.CompletedTask;

    /// <summary>
    /// Framework entry point called from BasePage.OnNavigatedTo.
    /// Creates a navigation scope, runs initialization, then calls OnNavigatedToAsync.
    /// </summary>
    internal async Task NotifyNavigatedToAsync(NavigationEventArgs e)
    {
        BeginNavigationScope();
        var ct = NavigationToken;
        await EnsureInitializedAsync(ct).ConfigureAwait(true);
        await OnNavigatedToAsync(e, ct).ConfigureAwait(true);
    }

    /// <summary>
    /// Framework entry point called from BasePage.OnNavigatedFrom.
    /// Calls OnNavigatedFromAsync, then ends the navigation scope.
    /// </summary>
    internal async Task NotifyNavigatedFromAsync(NavigationEventArgs e)
    {
        try
        {
            await OnNavigatedFromAsync(e, CancellationToken.None).ConfigureAwait(true);
        }
        finally
        {
            EndNavigationScope();
        }
    }
}
