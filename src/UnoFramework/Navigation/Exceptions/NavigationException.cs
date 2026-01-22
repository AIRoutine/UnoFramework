namespace UnoFramework.Navigation.Exceptions;

/// <summary>
/// Exception thrown when a navigation operation fails.
/// </summary>
public class NavigationException : Exception
{
    /// <summary>
    /// Creates a new navigation exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NavigationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates a new navigation exception with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NavigationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// The route that failed to navigate to, if applicable.
    /// </summary>
    public string? Route { get; init; }

    /// <summary>
    /// The region that failed to navigate within, if applicable.
    /// </summary>
    public string? RegionName { get; init; }
}
