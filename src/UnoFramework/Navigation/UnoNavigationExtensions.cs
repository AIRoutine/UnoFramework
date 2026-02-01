using UnoFramework.Navigation.Handlers;

namespace UnoFramework.Navigation;

/// <summary>
/// Extension methods for registering Uno navigation command handlers with Shiny.Mediator.
/// </summary>
public static class UnoNavigationExtensions
{
    /// <summary>
    /// Adds Uno navigation command handlers to the mediator configuration.
    /// This enables handling of <see cref="Commands.IUnoNavigationCommand"/> and
    /// <see cref="Commands.IUnoRegionNavigationCommand"/> via the mediator.
    /// </summary>
    /// <param name="builder">The Shiny Mediator builder.</param>
    /// <returns>The builder for chaining.</returns>
    public static ShinyMediatorBuilder AddUnoNavigationCommands(this ShinyMediatorBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        _ = builder.Services.AddSingleton(typeof(ICommandHandler<>), typeof(UnoNavigationCommandHandler<>));
        _ = builder.Services.AddSingleton(typeof(ICommandHandler<>), typeof(UnoRegionNavigationCommandHandler<>));
        return builder;
    }
}
