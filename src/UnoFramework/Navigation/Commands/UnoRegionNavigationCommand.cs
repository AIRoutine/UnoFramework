namespace UnoFramework.Navigation.Commands;

/// <summary>
/// Class-based implementation of <see cref="IUnoRegionNavigationCommand"/>.
/// Use this when you need a mutable command or inheritance.
/// </summary>
public class UnoRegionNavigationCommand : IUnoRegionNavigationCommand
{
    /// <summary>
    /// Creates a new region navigation command.
    /// </summary>
    /// <param name="regionName">The target region name.</param>
    /// <param name="viewName">The view to display in the region.</param>
    public UnoRegionNavigationCommand(string regionName, string viewName)
    {
        RegionName = regionName;
        ViewName = viewName;
    }

    /// <inheritdoc />
    public string RegionName { get; }

    /// <inheritdoc />
    public string ViewName { get; }

    /// <inheritdoc />
    public object? Data { get; set; }

    /// <inheritdoc />
    public required INavigator Navigator { get; set; }
}

/// <summary>
/// Record-based implementation of <see cref="IUnoRegionNavigationCommand"/>.
/// Preferred for immutable, declarative command definitions.
/// </summary>
/// <param name="RegionName">The target region name.</param>
/// <param name="ViewName">The view to display in the region.</param>
public record UnoRegionNavigationRecord(string RegionName, string ViewName) : IUnoRegionNavigationCommand
{
    /// <inheritdoc />
    public object? Data { get; init; }

    /// <inheritdoc />
    public required INavigator Navigator { get; init; }
}
