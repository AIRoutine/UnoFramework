using Microsoft.Extensions.Logging;
using Shiny.Mediator;

namespace UnoFramework.ViewModels;

/// <summary>
/// Common services injected into all ViewModels via BaseViewModel.
/// </summary>
/// <param name="LoggerFactory">The logger factory for creating typed loggers.</param>
/// <param name="Mediator">The mediator for publishing events.</param>
/// <param name="Navigator">The navigator for page navigation.</param>
[Service(UnoFrameworkService.PageLifetime, TryAdd = UnoFrameworkService.TryAdd)]
public record BaseServices(ILoggerFactory LoggerFactory, IMediator Mediator, INavigator Navigator);
