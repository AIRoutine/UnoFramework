namespace UnoFramework.Contracts.Navigation;

/// <summary>
/// Provides lifecycle callbacks for ViewModels during region navigation.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Called when the ViewModel is navigated to.
    /// </summary>
    /// <param name="parameter">The navigation parameter, if any.</param>
    void OnNavigatedTo(object? parameter);

    /// <summary>
    /// Called when navigating away from this ViewModel.
    /// </summary>
    void OnNavigatedFrom();
}
