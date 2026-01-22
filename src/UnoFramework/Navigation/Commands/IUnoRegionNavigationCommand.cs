namespace UnoFramework.Navigation.Commands;

/// <summary>
/// Interface for region-based navigation commands.
/// Use this for navigating within specific named regions (e.g., NavigationView content areas).
/// </summary>
public interface IUnoRegionNavigationCommand : Shiny.Mediator.ICommand
{
    /// <summary>
    /// The name of the target region to navigate within.
    /// This should match a Region.Name in XAML.
    /// </summary>
    string RegionName { get; }

    /// <summary>
    /// The view/route name to display in the region.
    /// </summary>
    string ViewName { get; }

    /// <summary>
    /// Optional data to pass to the target view/viewmodel.
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// The navigator instance to use for navigation.
    /// </summary>
    INavigator Navigator { get; }
}
