namespace UnoFramework.Configuration;

/// <summary>
/// Extension methods for UnoFramework DI registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds UnoFramework services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddUnoFramework(this IServiceCollection services)
    {
        _ = services.AddShinyServiceRegistry();
        return services;
    }
}
