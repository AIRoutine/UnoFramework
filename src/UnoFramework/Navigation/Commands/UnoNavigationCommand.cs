namespace UnoFramework.Navigation.Commands;

/// <summary>
/// Class-based implementation of <see cref="IUnoNavigationCommand"/>.
/// Use this when you need a mutable command or inheritance.
/// </summary>
/// <param name="route">The route to navigate to.</param>
public class UnoNavigationCommand(string route) : IUnoNavigationCommand
{
    /// <inheritdoc />
    public string Route { get; } = route;

    /// <inheritdoc />
    public string Qualifier { get; set; } = Qualifiers.None;

    /// <inheritdoc />
    public object? Data { get; set; }

    /// <inheritdoc />
    public required INavigator Navigator { get; set; }
}

/// <summary>
/// Record-based implementation of <see cref="IUnoNavigationCommand"/>.
/// Preferred for immutable, declarative command definitions.
/// </summary>
/// <param name="Route">The route to navigate to.</param>
public record UnoNavigationRecord(string Route) : IUnoNavigationCommand
{
    /// <inheritdoc />
    public string Qualifier { get; init; } = Qualifiers.None;

    /// <inheritdoc />
    public object? Data { get; init; }

    /// <inheritdoc />
    public required INavigator Navigator { get; init; }
}
