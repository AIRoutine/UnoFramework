namespace UnoFramework.Navigation.Commands;

/// <summary>
/// Interface for route-based navigation commands.
/// Commands implementing this interface will be handled by <see cref="Handlers.UnoNavigationCommandHandler{TCommand}"/>.
/// </summary>
public interface IUnoNavigationCommand : Shiny.Mediator.ICommand
{
    /// <summary>
    /// The route to navigate to. This should match a registered route name
    /// or a Region.Name in XAML.
    /// </summary>
    string Route { get; }

    /// <summary>
    /// The navigation qualifier (e.g., Qualifiers.None, Qualifiers.Dialog, Qualifiers.Nested).
    /// Use Qualifiers.Dialog for modal navigation.
    /// </summary>
    string Qualifier { get; }

    /// <summary>
    /// Optional data to pass to the target view/viewmodel.
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// The navigator instance to use for navigation.
    /// This should be obtained from the ViewModel's BaseServices.
    /// </summary>
    INavigator Navigator { get; }
}
