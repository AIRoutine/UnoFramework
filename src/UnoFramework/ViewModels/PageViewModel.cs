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
        OnNavigatingTo();
        return OnNavigatingToAsync(ct);
    }

    /// <summary>
    /// Called when the page is navigated away from.
    /// Override this method to save state or perform cleanup when leaving the page.
    /// </summary>
    /// <param name="e">Navigation event arguments.</param>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task OnNavigatedFromAsync(NavigationEventArgs e, CancellationToken ct = default)
    {
        OnNavigatingFrom();
        return OnNavigatingFromAsync(ct);
    }
}
