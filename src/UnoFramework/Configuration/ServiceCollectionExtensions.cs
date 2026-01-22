using Microsoft.Extensions.DependencyInjection;
using Shiny.Mediator.Infrastructure;
using UnoFramework.Mediator;
using UnoFramework.ViewModels;

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
        services.AddSingleton<IEventCollector, UnoEventCollector>();
        // BaseServices wird automatisch durch [Service] Attribute registriert
        return services;
    }
}
