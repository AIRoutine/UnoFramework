using Microsoft.Extensions.Logging;
using UnoFramework.Navigation.Commands;
using UnoFramework.Navigation.Exceptions;

namespace UnoFramework.Navigation.Handlers;

/// <summary>
/// Handles <see cref="IUnoNavigationCommand"/> by invoking Uno Extensions Navigation.
/// </summary>
/// <typeparam name="TCommand">The command type implementing <see cref="IUnoNavigationCommand"/>.</typeparam>
/// <param name="logger">The logger instance.</param>
[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class UnoNavigationCommandHandler<TCommand>(ILogger<UnoNavigationCommandHandler<TCommand>> logger) : ICommandHandler<TCommand>
    where TCommand : IUnoNavigationCommand
{
    /// <inheritdoc />
    public async Task Handle(TCommand command, IMediatorContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command.Navigator, nameof(command.Navigator));
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Route, nameof(command.Route));

        logger.LogDebug(
            "Navigating to route '{Route}' with qualifier '{Qualifier}'",
            command.Route,
            command.Qualifier);

        try
        {
            var response = await command.Navigator.NavigateRouteAsync(
                sender: this,
                route: command.Route,
                qualifier: command.Qualifier,
                data: command.Data,
                cancellation: cancellationToken).ConfigureAwait(true);

            if (response?.Success != true)
            {
                throw new NavigationException($"Navigation to '{command.Route}' failed")
                {
                    Route = command.Route
                };
            }

            logger.LogDebug("Successfully navigated to '{Route}'", command.Route);
        }
        catch (NavigationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Navigation to '{Route}' threw an exception", command.Route);
            throw new NavigationException($"Navigation to '{command.Route}' failed: {ex.Message}", ex)
            {
                Route = command.Route
            };
        }
    }
}
