namespace UnoFramework.ViewModels;

/// <summary>
/// ViewModel for Region-based navigation with lifecycle support.
/// Provides OnNavigatedToAsync and OnNavigatedFromAsync methods for region lifecycle.
/// These methods should be called from the View's Loaded/Unloaded events or from a BaseRegionControl.
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
    /// Call this from the View's Loaded event or from navigation logic.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task OnNavigatedToAsync(CancellationToken ct = default)
    {
        OnNavigatingTo();
        return OnNavigatingToAsync(ct);
    }

    /// <summary>
    /// Called when the region is navigated away from or becomes hidden.
    /// Override this method to save state or perform cleanup when leaving the region.
    /// Call this from the View's Unloaded event or from navigation logic.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    public virtual Task OnNavigatedFromAsync(CancellationToken ct = default)
    {
        OnNavigatingFrom();
        return OnNavigatingFromAsync(ct);
    }
}
