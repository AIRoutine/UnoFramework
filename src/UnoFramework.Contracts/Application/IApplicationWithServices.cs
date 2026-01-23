namespace UnoFramework.Contracts.Application;

/// <summary>
/// Interface for applications that provide access to dependency injection services.
/// </summary>
public interface IApplicationWithServices
{
    /// <summary>
    /// Gets the service provider for the application.
    /// </summary>
    IServiceProvider? Services { get; }
}
