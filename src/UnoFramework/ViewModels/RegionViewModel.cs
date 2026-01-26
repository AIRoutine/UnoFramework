namespace UnoFramework.ViewModels;

/// <summary>
/// ViewModel for Region-based navigation with lifecycle support.
/// Provides OnNavigatedToAsync and OnNavigatedFromAsync methods for region lifecycle.
/// These methods are called from the BaseRegionControl's Loaded/Unloaded events.
/// </summary>
public abstract partial class RegionViewModel : ViewModelBase
{
    /// <summary>
    /// Creates a new RegionViewModel.
    /// </summary>
    protected RegionViewModel(BaseServices baseServices) : base(baseServices)
    {
    }

    /// <summary>
    /// Called when the region is navigated to or becomes visible.
    /// Override this method to load data or perform initialization when the region becomes active.
    /// </summary>
    /// <param name="ct">Cancellation token that is cancelled when navigating away.</param>
    protected virtual Task OnNavigatedToAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the region is navigated away from or becomes hidden.
    /// Override this method to save state or perform cleanup when leaving the region.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    protected virtual Task OnNavigatedFromAsync(CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Framework entry point called from BaseRegionControl.OnLoaded.
    /// Creates a navigation scope, runs initialization, then calls OnNavigatedToAsync.
    /// </summary>
    internal async Task NotifyNavigatedToAsync()
    {
        BeginNavigationScope();
        var ct = NavigationToken;
        await EnsureInitializedAsync(ct);
        await OnNavigatedToAsync(ct);
    }

    /// <summary>
    /// Framework entry point called from BaseRegionControl.OnUnloaded.
    /// Calls OnNavigatedFromAsync, then ends the navigation scope.
    /// </summary>
    internal async Task NotifyNavigatedFromAsync()
    {
        try
        {
            await OnNavigatedFromAsync(CancellationToken.None);
        }
        finally
        {
            EndNavigationScope();
        }
    }
}
