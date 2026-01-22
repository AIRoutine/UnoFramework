using Microsoft.Extensions.Logging;
using Shiny.Mediator;
using UnoFramework.Navigation.Commands;
using UnoFramework.Navigation.Exceptions;

namespace UnoFramework.Navigation.Handlers;

/// <summary>
/// Handles <see cref="IUnoNavigationCommand"/> by invoking Uno Extensions Navigation.
/// </summary>
/// <typeparam name="TCommand">The command type implementing <see cref="IUnoNavigationCommand"/>.</typeparam>
[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class UnoNavigationCommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : IUnoNavigationCommand
{
    private readonly ILogger<UnoNavigationCommandHandler<TCommand>> _logger;

    public UnoNavigationCommandHandler(ILogger<UnoNavigationCommandHandler<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TCommand command, IMediatorContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command.Navigator, nameof(command.Navigator));
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Route, nameof(command.Route));

        _logger.LogDebug(
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
                cancellation: cancellationToken);

            if (response?.Success != true)
            {
                throw new NavigationException($"Navigation to '{command.Route}' failed")
                {
                    Route = command.Route
                };
            }

            _logger.LogDebug("Successfully navigated to '{Route}'", command.Route);
        }
        catch (NavigationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Navigation to '{Route}' threw an exception", command.Route);
            throw new NavigationException($"Navigation to '{command.Route}' failed: {ex.Message}", ex)
            {
                Route = command.Route
            };
        }
    }
}
