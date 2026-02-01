using Microsoft.Extensions.Logging;
using UnoFramework.Navigation.Commands;
using UnoFramework.Navigation.Exceptions;

namespace UnoFramework.Navigation.Handlers;

/// <summary>
/// Handles <see cref="IUnoRegionNavigationCommand"/> by navigating within a named region.
/// </summary>
/// <typeparam name="TCommand">The command type implementing <see cref="IUnoRegionNavigationCommand"/>.</typeparam>
/// <param name="logger">The logger instance.</param>
[Service(UnoFrameworkService.Lifetime, TryAdd = UnoFrameworkService.TryAdd)]
public class UnoRegionNavigationCommandHandler<TCommand>(ILogger<UnoRegionNavigationCommandHandler<TCommand>> logger) : ICommandHandler<TCommand>
    where TCommand : IUnoRegionNavigationCommand
{
    /// <inheritdoc />
    public async Task Handle(TCommand command, IMediatorContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command.Navigator, nameof(command.Navigator));
        ArgumentException.ThrowIfNullOrWhiteSpace(command.RegionName, nameof(command.RegionName));
        ArgumentException.ThrowIfNullOrWhiteSpace(command.ViewName, nameof(command.ViewName));

        // Build the route path: RegionName/ViewName
        var route = $"{command.RegionName}/{command.ViewName}";

        logger.LogDebug(
            "Navigating to view '{ViewName}' in region '{RegionName}' (route: '{Route}')",
            command.ViewName,
            command.RegionName,
            route);

        try
        {
            var response = await command.Navigator.NavigateRouteAsync(
                sender: this,
                route: route,
                data: command.Data,
                cancellation: cancellationToken).ConfigureAwait(true);

            if (response?.Success != true)
            {
                throw new NavigationException(
                    $"Navigation to view '{command.ViewName}' in region '{command.RegionName}' failed")
                {
                    Route = route,
                    RegionName = command.RegionName
                };
            }

            logger.LogDebug(
                "Successfully navigated to '{ViewName}' in region '{RegionName}'",
                command.ViewName,
                command.RegionName);
        }
        catch (NavigationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Navigation to '{ViewName}' in region '{RegionName}' threw an exception",
                command.ViewName,
                command.RegionName);

            throw new NavigationException(
                $"Navigation to view '{command.ViewName}' in region '{command.RegionName}' failed: {ex.Message}",
                ex)
            {
                Route = route,
                RegionName = command.RegionName
            };
        }
    }
}
