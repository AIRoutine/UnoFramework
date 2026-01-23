using Microsoft.UI.Xaml.Navigation;

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
    /// <param name="ct">Cancellation token.</param>
    public virtual Task OnNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Framework entry point that ensures initialization and navigation lifecycle ordering.
    /// Call this from the Page when navigating to the view.
    /// </summary>
    public async Task NotifyNavigatedToAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        OnNavigatingTo();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, NavigationToken);
        await EnsureInitializedAsync(linkedCts.Token);
        await OnNavigatingToAsync(linkedCts.Token);
        await OnNavigatedToAsync(e, linkedCts.Token);
    }

    /// <summary>
    /// Called when the page is navigated away from.
    /// Override this method to save state after navigation completes.
    /// </summary>
    /// <param name="e">Navigation event arguments.</param>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Framework entry point that triggers cleanup before navigation completes.
    /// </summary>
    public async Task NotifyNavigatingFromAsync(NavigatingCancelEventArgs e, CancellationToken ct = default)
    {
        _ = e;
        OnNavigatingFrom();
        await OnNavigatingFromAsync(ct);
    }

    /// <summary>
    /// Framework entry point that runs after navigation completes.
    /// </summary>
    public Task NotifyNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        return OnNavigatedFromAsync(e, ct);
    }
}
