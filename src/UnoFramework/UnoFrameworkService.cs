using Microsoft.Extensions.DependencyInjection;

namespace UnoFramework;

/// <summary>
/// UnoFramework-spezifische DI-Konstanten.
/// </summary>
public static class UnoFrameworkService
{
    /// <summary>
    /// Default: Singleton für Uno/Client-Apps.
    /// </summary>
    public const ServiceLifetime Lifetime = ServiceLifetime.Singleton;

    /// <summary>
    /// Scoped: Pro Page/Navigation-Scope.
    /// </summary>
    public const ServiceLifetime PageLifetime = ServiceLifetime.Scoped;

    /// <summary>
    /// Immer TryAdd verwenden.
    /// </summary>
    public const bool TryAdd = true;
}
