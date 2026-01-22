namespace UnoFramework.Contracts.Navigation;

/// <summary>
/// Defines a contract for a shell or host that manages named regions for navigation.
/// </summary>
public interface IRegionHost
{
    /// <summary>
    /// Gets the name of the primary content region.
    /// </summary>
    string ContentRegionName { get; }
}
